using System.Collections.Generic;
using System.Threading.Tasks;
using App.Data.Entities;
using App.Data.IDataRepository;
using App.Models.Result;
using App.Services.IServices;

namespace App.Services
{
    public class CommitService : ICommitService
    {
        private readonly ICommitRepo commitRepo;

        public CommitService(ICommitRepo commitRepo)
        {
            this.commitRepo = commitRepo;
        }

        public Commit GenerateCommitFromGitCommitInfo(LibGit2Sharp.Commit commit,
            Repository repository,
            Developer developer)
        {
            return new()
            {
                //Branch = currentBranch,
                Message = commit.MessageShort,
                Repository = repository,
                Sha = commit.Sha,
                Developer = developer,
                Date = commit.Author.When.UtcDateTime
            };
        }

        public int Add(Commit commit)
        {
            return this.commitRepo.Add(commit);
        }

        public int Update(Commit commit)
        {
            return this.commitRepo.Update(commit);
        }

        public int Update(List<Commit> commits)
        {
            return this.commitRepo.Update(commits);
        }

        public int Delete(Commit commit)
        {
            return this.commitRepo.Delete(commit);
        }

        public CommitSummaryList GetCommitSummaries(int repositoryId)
        {
            return this.commitRepo.GetCommitSummaries(repositoryId);
        }

        public async Task<Commit> GetLastScannedCommit(long repositoryId, int branchId)
        {
            return await this.commitRepo.FindLast(repositoryId, branchId);
        }

        public async Task<Commit> GetCommitById(int commitId)
        {
            return await this.commitRepo.GetCommitById(commitId);
        }
    }
}
