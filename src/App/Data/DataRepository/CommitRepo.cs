using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data.Entities;
using App.Data.IDataRepository;
using App.Models.Result;
using Microsoft.EntityFrameworkCore;

namespace App.Data.DataRepository
{
    public class CommitRepo : ICommitRepo
    {
        private readonly AppDbContext context;

        public CommitRepo(AppDbContext context)
        {
            this.context = context;
        }

        public int Add(Commit commit)
        {
            this.context.Commits.Add(commit);
            return this.context.SaveChanges();
        }

        public int Update(Commit commit)
        {
            this.context.Commits.Update(commit);
            return this.context.SaveChanges();
        }

        public int Update(List<Commit> commit)
        {
            this.context.Commits.UpdateRange(commit);
            return this.context.SaveChanges();
        }

        public int Delete(Commit commit)
        {
            this.context.Commits.Remove(commit);
            return this.context.SaveChanges();
        }

        public CommitSummaryList GetCommitSummaries(long repositoryId, int? branchId = null)
        {
            var response = new CommitSummaryList();

            var query = this.context.Commits
                .Include(dev => dev.Developer)
                .Include(branch => branch.Branch)
                .Where(r => r.Repository.RepositoryId == repositoryId).AsQueryable();

            if (branchId.HasValue)
            {
                query = query.Where(r => r.Branch.BranchId == (int)branchId);
            }

            response.CommitList = query.Select(c => new CommitSummary
            {
                Developer = c.Developer,
                Date = c.Date,
                Message = c.Message,
                Sha = c.Sha,
                CommitId = c.CommitId
            }).ToList();

            return response;
        }

        public async Task<Commit> GetCommitById(int commitId)
        {
            var query = this.context.Commits
                .Include(dev => dev.Developer)
                .Include(branch => branch.Branch)
                .Include(repository => repository.Repository)
                .Where(r => r.CommitId.Equals(commitId)).AsQueryable().AsNoTracking();


            return await query.Select(c => c).SingleOrDefaultAsync();
        }

        public async Task<Commit> FindLast(long repositoryId, int branchId)
        {
            var response = this.context.Commits
                .Where(r => r.Repository.RepositoryId == repositoryId)
                .Where(r => r.Branch.BranchId == branchId)
                .OrderBy(c => c.CommitId)
                .LastOrDefaultAsync();
            return await response;
        }
    }
}
