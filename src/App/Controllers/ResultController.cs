using System.Threading.Tasks;
using App.Models;
using App.Models.Result;
using App.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [Route("[controller]")]
    public class ResultController : Controller
    {
        private readonly IBranchService branchService;
        private readonly IRepositoryService repositoryService;
        private readonly IResultService resultService;

        public ResultController(IResultService resultServices,
            IRepositoryService repositoryService,
            IBranchService branchService)
        {
            this.resultService = resultServices;
            this.repositoryService = repositoryService;
            this.branchService = branchService;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            var result = await new RepositoryController(this.repositoryService, this.branchService).Index();
            return this.View(result);
        }

        [HttpPost("getresult")]
        public IActionResult GetResult(ResultsFilter resultsFilter)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(resultsFilter.BranchId);
            }

            if (resultsFilter.RepositoryId <= 0)
            {
                return this.RedirectToAction("Index");
            }


            var repository = this.repositoryService.GetById(resultsFilter.RepositoryId);
            var branch = this.branchService.GetById(resultsFilter.BranchId);

            var response = new ResultsResponse();
            response.RespositoryInfo = new RepositoryInfo
            {
                Name = repository.Name,
                RepositoryId = repository.RepositoryId,
                Url = repository.Url,
                Branch = branch.Name,
                BranchId = branch.BranchId
            };
            response.CommitSummary = this.resultService.Summary(resultsFilter);

            return this.Ok(response);
        }

        [HttpPost("getdetails")]
        public async Task<IActionResult> GetDetails([FromForm] int commitId)
        {
            var response = await this.resultService.CommitDetails(commitId);
            if (response.GetType() == typeof(DetailsOfCommitMetrics) || response.FileList != null)
            {
                return this.Ok(response);
            }

            return this.BadRequest("Cannot find details");
        }
    }
}
