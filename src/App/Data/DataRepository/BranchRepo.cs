using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data.Entities;
using App.Data.IDataRepository;
using Microsoft.EntityFrameworkCore;

namespace App.Data.DataRepository
{
    public class BranchRepo : IBranchRepo
    {
        private readonly AppDbContext context;

        public BranchRepo(AppDbContext context)
        {
            this.context = context;
        }

        public Branch GetByName(string name)
        {
            return this.context.Branches.Where(b => b.Name == name).SingleOrDefault();
        }

        public Branch GetById(int branchId)
        {
            return this.context.Branches.Where(b => b.BranchId == branchId).SingleOrDefault();
        }

        public int Add(Branch branch)
        {
            this.context.Branches.Add(branch);
            return this.context.SaveChanges();
        }

        public int Update(Branch branch)
        {
            this.context.Branches.Update(branch);
            return this.context.SaveChanges();
        }

        public int Delete(Branch branch)
        {
            this.context.Branches.Remove(branch);
            return this.context.SaveChanges();
        }

        public async Task<List<Branch>> GetAllByRepositoryId(long repositoryId)
        {
            return await this.context.Commits.Where(c => c.Repository.RepositoryId == repositoryId)
                .Select(c => c.Branch).Distinct().ToListAsync();
        }
    }
}
