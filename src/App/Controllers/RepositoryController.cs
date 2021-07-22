using System.Threading.Tasks;
using App.Models.Result;
using App.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [Route("[controller]")]
    public class RepositoryController : Controller
    {
        private readonly IBranchService branchService;
        private readonly IRepositoryService repositoryService;

        public RepositoryController(IRepositoryService repositoryService, IBranchService branchService)
        {
            this.repositoryService = repositoryService;
            this.branchService = branchService;
        }

        [HttpPost]
        public async Task<ListRepositories> Index()
        {
            var response = new ListRepositories();
            response.Repositories = await this.repositoryService.GetAllAsync();

            return response;
        }

        [HttpGet("getrepositorybranches")]
        public async Task<IActionResult> GetRepositoryBranches([FromQuery] long? repositoryId = null)
        {
            if (!repositoryId.HasValue)
            {
                return this.BadRequest("No data");
            }

            var Branches = await this.branchService.GetAllByRepositoryId((int)repositoryId);
            return this.Ok(Branches);
        }
    }
}
