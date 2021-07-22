using System.Collections.Generic;
using System.Threading.Tasks;
using App.Models;
using LibGit2Sharp;
using Repository = App.Data.Entities.Repository;

namespace App.Services.IServices
{
    public interface IRepositoryService
    {
        int Update(Repository repository);
        int Delete(Repository repository);
        int Add(Repository repository);
        Repository Create(RepositoryForm repositoryForm);
        Repository GetById(long repositoryId);
        Task<List<Repository>> GetAllAsync();
        IRepository CloneRepository(string repositoryUrl, string branch = null);
        void DeleteRepositoryDirectory(string path);
        RepositoryType GetRepositoryType(string path);
        string GetUserNameFromRepositoryUrl(string repositoryUrl);
        string GetRepositoryNameFromRepositoryUrl(string repositoryUrl);
        string CreatePathToRepository(string repositoryURL);
    }
}
