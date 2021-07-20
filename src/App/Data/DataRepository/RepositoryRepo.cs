using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data.Entities;
using App.Data.IDataRepository;
using Microsoft.EntityFrameworkCore;

namespace App.Data.DataRepository
{
    public class RepositoryRepo : IRepositoryRepo
    {
        private readonly AppDbContext context;

        public RepositoryRepo(AppDbContext context)
        {
            this.context = context;
        }

        public Repository GetById(long id)
        {
            return this.context.Repositories.AsNoTracking().FirstOrDefault(r => r.RepositoryId == id);
        }

        public int Add(Repository repository)
        {
            this.context.Repositories.Add(repository);
            return this.context.SaveChanges();
        }

        public int Update(Repository repository)
        {
            this.context.Repositories.Update(repository);
            return this.context.SaveChanges();
        }

        public int Delete(Repository repository)
        {
            this.context.Repositories.Remove(repository);
            return this.context.SaveChanges();
        }

        public async Task<List<Repository>> GetAllAsync()
        {
            var result = this.context.Repositories.AsQueryable();
            return await result.AsNoTracking().ToListAsync();
        }
    }
}
