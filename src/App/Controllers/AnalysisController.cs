using System;
using System.Threading.Tasks;
using App.Data.Entities;
using App.Models;
using App.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [Route("[controller]")]
    [Controller]
    public class AnalysisController : Controller
    {
        private readonly IBranchService branchService;
        private readonly IRepositoryService repositoryService;
        private readonly ISonarQubeScanner sonarQubeScanner;

        public AnalysisController(IRepositoryService repositoryService,
            ISonarQubeScanner sonarQubeScanner,
            IBranchService branchService)
        {
            this.repositoryService = repositoryService;
            this.sonarQubeScanner = sonarQubeScanner;
            this.branchService = branchService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost("Analysis")]
        public async Task<IActionResult> Analysis(RepositoryForm repositoryForm)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var repository = this.repositoryService.Create(repositoryForm);

            try
            {
                await this.sonarQubeScanner.ScanRepositoryAsync(repository, repositoryForm.Branch);
            }
            catch (ApplicationException applicationException)
            {
                return this.BadRequest(applicationException.Message);
            }

            return this.Ok();
        }

        [HttpPost("AnalysisUpdate")]
        public async Task<IActionResult> AnalysisUpdate(StoredRepository storedRepository)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var repository = this.repositoryService.GetById(storedRepository.RepositoryId);
            Branch branch = null;
            if (storedRepository.BranchId != null)
            {
                branch = this.branchService.GetById((int)storedRepository.BranchId);
            }
            else if (storedRepository.Branch != string.Empty)
            {
                branch = this.branchService.GetByName(storedRepository.Branch);
            }

            if (branch == null)
            {
                branch = this.branchService.CreateBranch(storedRepository.Branch);
            }


            try
            {
                await this.sonarQubeScanner.ScanRepositorySinceLastScannedCommit(repository, branch);
            }
            catch (ApplicationException applicationException)
            {
                return this.BadRequest(applicationException.Message);
            }

            return this.Ok("Analysis completed");
        }
    }
}
