using App.Data.Entities;
using Commit = LibGit2Sharp.Commit;

namespace App.Services.IServices
{
    public interface IDeveloperService
    {
        Developer CreateDeveloperFromGitCommit(Commit commit);
        int Add(Developer developer);
        int Update(Developer developer);
        int Delete(Developer developer);
        Developer GetByEmail(string email);
    }
}
