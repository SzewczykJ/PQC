using System.Collections.Generic;
using System.Threading.Tasks;
using App.Data.Entities;

namespace App.Services.IServices
{
    public interface IBranchService
    {
        int Add(Branch branch);
        int Update(Branch branch);
        int Delete(Branch branch);
        Branch CreateBranch(string branchName);
        Branch GetByName(string branchName);
        Branch GetById(int branchId);
        Task<List<Branch>> GetAllByRepositoryId(long repositoryId);
    }
}
