using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data.IDataRepository;
using App.Models;
using Microsoft.EntityFrameworkCore;
using File = App.Data.Entities.File;

namespace App.Data.DataRepository
{
    public class FileRepo : IFileRepo
    {
        private readonly AppDbContext context;

        public FileRepo(AppDbContext context)
        {
            this.context = context;
        }

        public File FindById(int id)
        {
            return this.context.Files
                .Select(f => f).FirstOrDefault(f => f.FileId.Equals(id));
        }

        public int Add(File file)
        {
            this.context.Files.Add(file);
            return this.context.SaveChanges();
        }

        public int Update(File file)
        {
            this.context.Files.Update(file);
            return this.context.SaveChanges();
        }

        public int Delete(File file)
        {
            this.context.Files.Remove(file);
            return this.context.SaveChanges();
        }

        public async Task<List<File>> GetListAsync(int? repositoryId = null)
        {
            var result = this.context.Files
                .Include(c => c.Commit)
                .ThenInclude(r => r.Repository)
                .OrderBy(r => r.Commit.Repository.Name)
                .Include(b => b.Commit.Branch)
                .OrderBy(c => c.Commit.Branch.Name)
                .AsNoTracking()
                .AsQueryable();
            if (repositoryId.HasValue)
            {
                result = result.Where(r => r.Commit.Repository.RepositoryId == repositoryId);
            }

            return await result.ToListAsync();
        }
        public async Task<IEnumerable<FileDetailsDtos>> GetListOfFilesWithDetails(int commitId)
        {
            var result = this.context.Files
                .Where(c => c.Commit.CommitId.Equals(commitId))
                .Select(file => new FileDetailsDtos
                {
                    FileId = file.FileId,
                    Status = file.Status,
                    Name = file.FileDetail.Name,
                    SHA = file.SHA,
                    Extension = file.FileDetail.Extension,
                    Language = file.FileDetail.Language.Name,
                    FullPath = file.FileDetail.FullPath,
                    Metric = file.Metric
                })
                .OrderBy(f => f.FullPath)
                .ToListAsync();
            return await result;
        }
    }
}
