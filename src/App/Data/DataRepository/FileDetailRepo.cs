using System.Linq;
using App.Data.Entities;
using App.Data.IDataRepository;

namespace App.Data.DataRepository
{
    public class FileDetailRepo : IFileDetailRepo
    {
        private readonly AppDbContext context;

        public FileDetailRepo(AppDbContext context)
        {
            this.context = context;
        }

        public FileDetail FindByPath(string pathName)
        {
            return this.context.FileDetails.SingleOrDefault(p => p.FullPath == pathName);
        }

        public int Add(FileDetail fileDetail)
        {
            this.context.FileDetails.Add(fileDetail);
            return this.context.SaveChanges();
        }

        public int Update(FileDetail fileDetail)
        {
            this.context.FileDetails.Update(fileDetail);
            return this.context.SaveChanges();
        }

        public int Delete(FileDetail fileDetail)
        {
            this.context.FileDetails.Remove(fileDetail);
            return this.context.SaveChanges();
        }
    }
}
