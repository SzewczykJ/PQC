using System.Collections.Generic;
using System.Threading.Tasks;
using App.Data.Entities;

namespace App.Data.IDataRepository
{
    public interface IBranchRepo
    {
        Branch GetByName(string name);
        Branch GetById(int branchId);
        int Add(Branch branch);
        int Update(Branch branch);
        int Delete(Branch branch);
        Task<List<Branch>> GetAllByRepositoryId(long repositoryId);
    }
}
