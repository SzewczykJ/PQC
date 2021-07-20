using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using App.Models;
using App.Services.IServices;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Branch = App.Data.Entities.Branch;
using Commit = LibGit2Sharp.Commit;
using Repository = App.Data.Entities.Repository;

namespace App.Services
{
    public class SonarQubeScanner : ISonarQubeScanner
    {
        private readonly IBranchService branchService;
        private readonly ICommitService commitService;
        private readonly IDeveloperService developerService;
        private readonly ILogger logger;
        private readonly IRepositoryService repositoryService;

        private readonly string SONAR_URL;
        private readonly ISonarQubeClient sonarQubeClient;
        private readonly ISonarQubeService sonarQubeService;


        public SonarQubeScanner(ISonarQubeClient sonarQubeClient,
            IRepositoryService repositoryService,
            ISonarQubeService sonarQubeService,
            ICommitService commitService,
            IDeveloperService developerService,
            IBranchService branchService,
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            this.sonarQubeClient = sonarQubeClient;
            this.repositoryService = repositoryService;
            this.sonarQubeService = sonarQubeService;
            this.commitService = commitService;
            this.developerService = developerService;
            this.branchService = branchService;
            this.SONAR_URL = configuration["SonarQubeUrl"];
            this.logger = loggerFactory.CreateLogger("MySpecialLogger");
            ;
        }

        public async Task ScanRepositoryAsync(Repository repository, string branch = null)
        {
            var repositoryUrl = repository.Url;

            using (var clonedRepository = this.repositoryService.CloneRepository(repositoryUrl, branch))
            {
                var x = clonedRepository.Branches[branch];
                if (!string.IsNullOrEmpty(branch))
                {
                    Commands.Checkout(clonedRepository,
                        clonedRepository.Branches.FirstOrDefault(b => b.FriendlyName.Equals(branch)));
                }

                var branchName = clonedRepository.Head.FriendlyName;

                await this.ScanAllCommitsFromRepositoryAsync(clonedRepository.Commits.ToArray(),
                    repository, repositoryUrl, clonedRepository, branchName);
            }


            this.repositoryService.DeleteRepositoryDirectory(
                this.repositoryService.CreatePathToRepository(repositoryUrl));
        }

        public async Task ScanRepositorySinceLastScannedCommit(Repository repository, Branch branch)
        {
            var repositoryUrl = repository.Url;
            var lastScannedCommit =
                await this.commitService.GetLastScannedCommit(repository.RepositoryId, branch.BranchId);

            using (var clonedRepository = this.repositoryService.CloneRepository(repositoryUrl, branch.Name))
            {
                var x = clonedRepository.Branches[branch.Name];
                if (!string.IsNullOrEmpty(branch.Name))
                {
                    Commands.Checkout(clonedRepository,
                        clonedRepository.Branches.FirstOrDefault(b => b.FriendlyName.Equals(branch.Name)));
                }

                var branchName = clonedRepository.Head.FriendlyName;

                var commits = clonedRepository.Commits;
                if (lastScannedCommit != null)
                {
                    var filter = new CommitFilter {IncludeReachableFrom = lastScannedCommit.Sha};

                    commits = (IQueryableCommitLog)clonedRepository.Commits.QueryBy(filter);
                }


                await this.ScanAllCommitsFromRepositoryAsync(commits.ToArray(),
                    repository, repositoryUrl, clonedRepository, branchName);
            }

            this.repositoryService.DeleteRepositoryDirectory(
                this.repositoryService.CreatePathToRepository(repositoryUrl));
        }

        public async Task ScanAllCommitsFromRepositoryAsync(Commit[] commits,
            Repository sonarRepository,
            string repositoryURL,
            IRepository repository,
            string branchName
        )
        {
            var userName = this.repositoryService.GetUserNameFromRepositoryUrl(repositoryURL);
            var repositoryName = this.repositoryService.GetRepositoryNameFromRepositoryUrl(repositoryURL);
            var path = this.repositoryService.CreatePathToRepository(repositoryURL);
            var repositoryType = this.repositoryService.GetRepositoryType(path);
            var project = await this.sonarQubeClient.CreateProject($"{userName}-{repositoryName}");
            var token = await this.sonarQubeClient.GenerateToken($"{userName}-{repositoryName}");
            var branch = this.branchService.CreateBranch(branchName);

            for (var i = commits.Length - 1; i >= 0; i--)
            {
                var actualCommit = commits[i];

                var developerOfCommit = this.developerService.CreateDeveloperFromGitCommit(actualCommit);
                var commitToSave = this.commitService.GenerateCommitFromGitCommitInfo(actualCommit, sonarRepository,
                    developerOfCommit);
                commitToSave.Branch = branch;
                this.commitService.Update(commitToSave);

                Dictionary<string, CommitChanges> commitChanges;
                if (i == commits.Length - 1)
                {
                    commitChanges = this.GetChangedFilesFromCommit(null, actualCommit.Tree, repository);
                }
                else
                {
                    commitChanges = this.GetChangedFilesFromCommit(actualCommit.Parents.First().Tree, actualCommit.Tree,
                        repository);
                }

                var watchFullScan = Stopwatch.StartNew();
                this.CheckoutCommit(actualCommit.Sha, path);

                var watchScanner = Stopwatch.StartNew();
                this.RunScanner(project, token, path, repositoryType, string.Join(",", commitChanges.Keys));
                watchScanner.Stop();

                await this.sonarQubeService.SaveDataFromRepositoryAsync(project.Key, commitToSave,
                    commitChanges);

                watchFullScan.Stop();
                var elapsedMs1 = watchScanner.ElapsedMilliseconds;

                var elapsedMs2 = watchFullScan.ElapsedMilliseconds;
                this.logger.LogDebug("{count} {t1} {t2} {t3} {t4}", commitChanges.Count, elapsedMs1, elapsedMs1,
                    TimeSpan.FromMilliseconds(elapsedMs1).TotalSeconds,
                    TimeSpan.FromMilliseconds(elapsedMs2).TotalSeconds);
            }

            await this.sonarQubeClient.RevokeToken(token.Name);
            await this.sonarQubeClient.DeleteProject(project.Key);
        }


        public int CheckoutCommit(string commitSHA, string workingDirectory)
        {
            return this.Execute("git", $"checkout -f {commitSHA}", workingDirectory);
        }

        private Dictionary<string, CommitChanges> GetChangedFilesFromCommit(Tree oldTree,
            Tree newTree,
            IRepository repository)
        {
            var commitChanges = new Dictionary<string, CommitChanges>();
            if (oldTree == null && newTree == null)
            {
                return commitChanges;
            }

            foreach (var changes in repository.Diff.Compare<TreeChanges>(oldTree, newTree))
            {
                commitChanges.Add(changes.Path, new CommitChanges {Status = changes.Status, SHA = changes.Oid.Sha});
            }

            return commitChanges;
        }


        private void RunScanner(
            Project createProject,
            ProjectToken createToken,
            string path,
            RepositoryType repositoryType,
            string changedFiles
        )
        {
            switch (repositoryType)
            {
                case RepositoryType.MAVEN:
                {
                    this.ScanMavenProject(createProject.Key, createToken.Token, path, changedFiles);
                    break;
                }
                case RepositoryType.MS:
                {
                    this.ScanDotnetProject(createProject.Key, createToken.Token, path, changedFiles);
                    break;
                }
                case RepositoryType.OTHER:
                {
                    this.ScanOtherType(createProject.Key, createToken.Token, path, changedFiles);
                    break;
                }
            }
        }

        private void ScanMavenProject(string projectKey,
            string loginToken,
            string path,
            string changedFiles)
        {
            this.BuildMaven(path);
            this.ScanMaven(projectKey, loginToken, path, changedFiles);
        }


        private void ScanDotnetProject(string projectkey,
            string loginToken,
            string path,
            string changedFiles)
        {
            this.StartDotnetScanner(projectkey, loginToken, path, changedFiles);
            this.DotnetRestore(path);
            this.RebuildDotnetProject(path);
            // RunDotnetTests(path);
            this.EndDotnetScanner(loginToken, path);
        }

        private int BuildMaven(string workingDirectory)
        {
            return this.Execute("mvn", @"clean install ""-DskipTests""", workingDirectory);
        }

        private int ScanMaven(string projectKey,
            string loginToken,
            string workingDirectory,
            string changedFiles)
        {
            if (changedFiles == null)
            {
                return this.Execute("mvn",
                    $@"sonar:sonar -Dsonar.projectKey={projectKey} -Dsonar.host.url={this.SONAR_URL} -Dsonar.login={loginToken}",
                    workingDirectory);
            }

            return this.Execute("mvn",
                $@"sonar:sonar -Dsonar.projectKey={projectKey} -Dsonar.host.url={this.SONAR_URL} -Dsonar.login={loginToken} -Dsonar.inclusions={changedFiles}",
                workingDirectory);
        }

        private int StartDotnetScanner(string projectKey,
            string loginToken,
            string workingDirectory,
            string changedFiles)
        {
            if (changedFiles == null)
            {
                return this.Execute("/bin/bash",
                    $@"-c ""dotnet sonarscanner begin /k:""{projectKey}"" /d:sonar.host.url={this.SONAR_URL} /d:sonar.login=""{loginToken}"" ",
                    workingDirectory);
            }

            return this.Execute("/bin/bash",
                @"-c ""dotnet sonarscanner begin /k:""{projectKey}"" /d:sonar.host.url={this.SONAR_URL} /d:sonar
                .login=""{loginToken}"" /d:sonar.inclusions=""{changedFiles}""""",
                workingDirectory);
        }

        private int DotnetRestore(string workingDirectory)
        {
            return this.Execute("/bin/bash", @"-c ""dotnet restore""", workingDirectory);
        }

        private int RebuildDotnetProject(string workingDirectory)
        {
            return this.Execute("/bin/bash", @"-c ""dotnet build""", workingDirectory);
        }

        private int RunDotnetTests(string workingDirectory)
        {
            return this.Execute("/bin/bash", $@"-c ""dotnet test --no-build
                /p:CollectCoverage=true
                /p:CoverletOutputFormat=""opencover""
                /p:CoverletOutput=""{workingDirectory}/TestResults/""
                /p:Exclude=""[xunit.runner.*]*""
                --logger=""trx"" --results-directory=""{workingDirectory}/TestResults/""", workingDirectory);
        }

        private int EndDotnetScanner(string loginToken, string workingDirectory)
        {
            return this.Execute("/bin/bash", $@"-c ""dotnet sonarscanner end /d:sonar.login=""{loginToken}""""",
                workingDirectory);
        }

        private int ScanOtherType(string projectKey,
            string loginToken,
            string workingDirectory,
            string changedFiles)
        {
            if (changedFiles == null)
            {
                return this.Execute("/bin/bash",
                    $@"-c ""sonar-scanner -Dsonar.projectKey={projectKey} -Dsonar.sources=. -Dsonar.host.url={this.SONAR_URL} -Dsonar.login={loginToken}""",
                    workingDirectory);
            }


            return this.Execute("/bin/bash",
                $@"-c ""sonar-scanner -Dsonar.projectKey={projectKey} -Dsonar.sources=. -Dsonar.host.url={this.SONAR_URL} -Dsonar.login={loginToken} -Dsonar.inclusions={changedFiles}""",
                workingDirectory);
        }

        private int Execute(string command, string argument, string workingDirectory)
        {
            var process = new Process();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = argument;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
            var err = process.StandardError.ReadToEnd();
            Console.WriteLine(err);

            process.WaitForExit();
            var exitCode = process.ExitCode;
            process.Close();

            return exitCode;
        }
    }
}
