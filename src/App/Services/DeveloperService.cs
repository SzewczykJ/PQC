using App.Data.Entities;
using App.Data.IDataRepository;
using App.Services.IServices;
using Commit = LibGit2Sharp.Commit;

namespace App.Services
{
    public class DeveloperService : IDeveloperService
    {
        private readonly IDeveloperRepo developerRepo;

        public DeveloperService(IDeveloperRepo developerRepo)
        {
            this.developerRepo = developerRepo;
        }

        public Developer CreateDeveloperFromGitCommit(Commit commit)
        {
            var developer = this.developerRepo.GetByEmail(commit.Committer.Email);
            if (developer == null)
            {
                return new Developer {Username = commit.Committer.Name, Email = commit.Committer.Email};
            }

            return developer;
        }

        public int Add(Developer developer)
        {
            return this.developerRepo.Add(developer);
        }

        public int Update(Developer developer)
        {
            return this.developerRepo.Update(developer);
        }

        public int Delete(Developer developer)
        {
            return this.developerRepo.Delete(developer);
        }

        public Developer GetByEmail(string email)
        {
            return this.developerRepo.GetByEmail(email);
        }
    }
}
