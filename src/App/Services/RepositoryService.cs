using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using App.Data.IDataRepository;
using App.Models;
using App.Services.IServices;
using LibGit2Sharp;
using Repository = App.Data.Entities.Repository;

namespace App.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly IRepositoryRepo repositoryRepo;

        public RepositoryService(IRepositoryRepo repositoryRepo)
        {
            this.repositoryRepo = repositoryRepo;
        }

        public int Update(Repository repository)
        {
            return this.repositoryRepo.Update(repository);
        }

        public int Delete(Repository repository)
        {
            return this.repositoryRepo.Delete(repository);
        }

        public int Add(Repository repository)
        {
            return this.repositoryRepo.Add(repository);
        }

        public Repository Create(RepositoryForm repositoryForm)
        {
            if (repositoryForm == null)
            {
                return null;
            }

            var repository = new Repository
            {
                Name = repositoryForm.Name,
                Url = repositoryForm.Url,
                FullName = this.GetRepositoryNameFromRepositoryUrl(repositoryForm.Url)
            };

            this.Add(repository);

            return repository;
        }

        public Repository GetById(long repositoryId)
        {
            return this.repositoryRepo.GetById(repositoryId);
        }

        public async Task<List<Repository>> GetAllAsync()
        {
            return await this.repositoryRepo.GetAllAsync();
        }

        public IRepository CloneRepository(string repositoryUrl, string branch = null)
        {
            try
            {
                if (branch != null && branch != string.Empty)
                {
                    return new LibGit2Sharp.Repository(LibGit2Sharp.Repository.Clone(repositoryUrl,
                        this.CreatePathToRepository(repositoryUrl),
                        new CloneOptions {BranchName = branch})
                    );
                }

                return new LibGit2Sharp.Repository(LibGit2Sharp.Repository.Clone(repositoryUrl,
                    this.CreatePathToRepository(repositoryUrl)));
            }
            catch (Exception ex)
            {
                if (ex is NameConflictException)
                {
                    this.DeleteRepositoryDirectory(this.CreatePathToRepository(repositoryUrl));
                    return new LibGit2Sharp.Repository(LibGit2Sharp.Repository.Clone(repositoryUrl,
                        this.CreatePathToRepository(repositoryUrl)));
                }

                throw new ApplicationException(
                    "Can not clone the repository. Check is it set to the public repository and try again. ", ex);
            }
        }


        public void DeleteRepositoryDirectory(string path)
        {
            foreach (var subDir in Directory.EnumerateDirectories(path))
            {
                this.DeleteRepositoryDirectory(subDir);
            }

            foreach (var fileName in Directory.EnumerateFiles(path))
            {
                var fileInfo = new FileInfo(fileName);
                fileInfo.Attributes = FileAttributes.Normal;
                fileInfo.Delete();
            }

            Directory.Delete(path);
        }

        public RepositoryType GetRepositoryType(string path)
        {
            foreach (var file in Directory.EnumerateFiles(path))
            {
                if (file.ToLower().Contains("pom.xml"))
                {
                    return RepositoryType.MAVEN;
                }

                if (file.ToLower().Contains(".gradle"))
                {
                    return RepositoryType.GRADLE;
                }

                if (file.ToLower().Contains(".sln"))
                {
                    return RepositoryType.MS;
                }
            }

            return RepositoryType.OTHER;
        }

        public string GetUserNameFromRepositoryUrl(string repositoryUrl)
        {
            //example url => https://github.com/github/training-kit/
            var split = repositoryUrl.Split("/");
            return split[3];
        }

        public string GetRepositoryNameFromRepositoryUrl(string repositoryUrl)
        {
            //example url => https://github.com/github/training-kit/
            var split = repositoryUrl.Split("/");
            return split[4];
        }

        public string CreatePathToRepository(string repositoryURL)
        {
            var userName = this.GetUserNameFromRepositoryUrl(repositoryURL);
            var repositoryName = this.GetRepositoryNameFromRepositoryUrl(repositoryURL);

            return $"downloadedRepositories/{userName}/{repositoryName}";
        }
    }
}
