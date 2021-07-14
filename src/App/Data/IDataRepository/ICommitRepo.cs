using System.Collections.Generic;
using System.Threading.Tasks;
using App.Data.Entities;
using App.Models.Result;

namespace App.Data.IDataRepository
{
    public interface ICommitRepo
    {
        int Add(Commit commit);
        int Update(Commit commit);
        int Update(List<Commit> commit);
        int Delete(Commit commit);
        CommitSummaryList GetCommitSummaries(long repositoryId, int? branchId = null);
        Task<Commit> GetCommitById(int commitId);
        Task<Commit> FindLast(long repositoryId, int branchId);
    }
}
