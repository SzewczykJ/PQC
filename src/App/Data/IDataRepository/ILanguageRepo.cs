using System.Collections.Generic;
using App.Data.Entities;

namespace App.Data.IDataRepository
{
    public interface ILanguageRepo
    {
        Language FindByName(string name);
        List<Language> GetAll();
    }
}
