using System.Collections.Generic;
using System.Threading.Tasks;
using App.Data.Entities;
using App.Models;

namespace App.Services.IServices
{
    public interface ISonarQubeService
    {
        Task SaveDataFromRepositoryAsync(string projectName,
            Commit sonarCommit,
            Dictionary<string, CommitChanges> commitChanges);
    }
}
