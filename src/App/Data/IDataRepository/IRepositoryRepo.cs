using System.Collections.Generic;
using System.Threading.Tasks;
using App.Data.Entities;

namespace App.Data.IDataRepository
{
    public interface IRepositoryRepo
    {
        Repository GetById(long id);
        int Add(Repository repository);
        int Update(Repository repository);
        int Delete(Repository repository);
        Task<List<Repository>> GetAllAsync();
    }
}
