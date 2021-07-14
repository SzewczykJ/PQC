using System.Collections.Generic;
using System.Threading.Tasks;
using App.Data.Entities;
using App.Data.IDataRepository;
using App.Services.IServices;


namespace App.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepo branchRepo;

        public BranchService(IBranchRepo branchRepo)
        {
            this.branchRepo = branchRepo;
        }

        public int Add(Branch branch)
        {
            return this.branchRepo.Add(branch);
        }

        public int Update(Branch branch)
        {
            return this.branchRepo.Update(branch);
        }

        public int Delete(Branch branch)
        {
            return this.branchRepo.Delete(branch);
        }

        public Branch CreateBranch(string branchName)
        {
            var storedBranch = this.branchRepo.GetByName(branchName);

            if (storedBranch == null)
            {
                storedBranch = new Branch {Name = branchName};
                this.Add(storedBranch);
            }

            return storedBranch;
        }

        public Branch GetByName(string branchName)
        {
            return this.branchRepo.GetByName(branchName);
        }

        public Branch GetById(int branchId)
        {
            return this.branchRepo.GetById(branchId);
        }

        public Task<List<Branch>> GetAllByRepositoryId(long repositoryId)
        {
            return this.branchRepo.GetAllByRepositoryId(repositoryId);
        }
    }
}
