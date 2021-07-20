using System.Threading.Tasks;
using LibGit2Sharp;
using Branch = App.Data.Entities.Branch;
using Commit = LibGit2Sharp.Commit;
using Repository = App.Data.Entities.Repository;

namespace App.Services.IServices
{
    public interface ISonarQubeScanner
    {
        Task ScanRepositoryAsync(Repository repository, string branch = null);
        Task ScanRepositorySinceLastScannedCommit(Repository repository, Branch branch);

        Task ScanAllCommitsFromRepositoryAsync(Commit[] commits,
            Repository sonarRepository,
            string repositoryURL,
            IRepository repository,
            string branchName);

        int CheckoutCommit(string commitSHA, string workingDirectory);
    }
}
