using System.Collections.Generic;
using System.Linq;
using App.Data.Entities;
using App.Data.IDataRepository;

namespace App.Data.DataRepository
{
    public class LanguageRepo : ILanguageRepo
    {
        private readonly AppDbContext context;

        public LanguageRepo(AppDbContext context)
        {
            this.context = context;
        }

        public Language FindByName(string name)
        {
            return this.context.Languages.FirstOrDefault(l => l.Name.Equals(name));
        }

        public List<Language> GetAll()
        {
            return this.context.Languages.ToList();
        }
    }
}
