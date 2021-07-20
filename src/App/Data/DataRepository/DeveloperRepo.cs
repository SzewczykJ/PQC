using System.Linq;
using App.Data.Entities;
using App.Data.IDataRepository;

namespace App.Data.DataRepository
{
    public class DeveloperRepo : IDeveloperRepo
    {
        private readonly AppDbContext context;

        public DeveloperRepo(AppDbContext context)
        {
            this.context = context;
        }

        public int Add(Developer developer)
        {
            this.context.Developers.Add(developer);
            return this.context.SaveChanges();
        }

        public int Update(Developer developer)
        {
            this.context.Developers.Update(developer);
            return this.context.SaveChanges();
        }

        public int Delete(Developer developer)
        {
            this.context.Developers.Remove(developer);
            return this.context.SaveChanges();
        }

        public Developer GetByEmail(string email)
        {
            return this.context.Developers.SingleOrDefault(d => d.Email == email);
        }
    }
}
