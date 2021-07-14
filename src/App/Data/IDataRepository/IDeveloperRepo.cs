using App.Data.Entities;

namespace App.Data.IDataRepository
{
    public interface IDeveloperRepo
    {
        int Add(Developer developer);
        int Update(Developer developer);
        int Delete(Developer developer);
        Developer GetByEmail(string email);
    }
}
