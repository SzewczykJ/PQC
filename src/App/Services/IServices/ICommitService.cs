using System.Collections.Generic;
using System.Threading.Tasks;
using App.Data.Entities;
using App.Models.Result;

namespace App.Services.IServices
{
    public interface ICommitService
    {
        Commit GenerateCommitFromGitCommitInfo(LibGit2Sharp.Commit commit, Repository repository, Developer developer);
        int Add(Commit commit);
        int Update(Commit commit);
        int Update(List<Commit> commits);
        int Delete(Commit commit);
        CommitSummaryList GetCommitSummaries(int repositoryId);
        Task<Commit> GetLastScannedCommit(long repositoryId, int branchId);
        Task<Commit> GetCommitById(int commitId);
    }
}
