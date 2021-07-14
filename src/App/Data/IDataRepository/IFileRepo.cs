using System.Collections.Generic;
using System.Threading.Tasks;
using File = App.Data.Entities.File;

namespace App.Data.IDataRepository
{
    public interface IFileRepo
    {
        File FindById(int id);
        int Add(File file);
        int Update(File file);
        int Delete(File file);
        Task<List<File>> GetListAsync(int? repositoryId = null);
    }
}
