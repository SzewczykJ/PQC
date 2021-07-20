using System.Net;
using System.Threading.Tasks;
using App.Models;

namespace App.Services.IServices
{
    public interface ISonarQubeClient
    {
        Task<Root> GetMetricsForFile(string fileKey);
        Task<ProjectTree> GetProjectTree(string projectName);
        Task<Project> CreateProject(string projectName);
        Task<HttpStatusCode> DeleteProject(string projectName);
        Task<ProjectToken> GenerateToken(string tokenName);
        Task<HttpStatusCode> RevokeToken(string tokenName);
    }
}
