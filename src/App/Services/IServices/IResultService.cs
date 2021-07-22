using System.Threading.Tasks;
using App.Models;
using App.Models.Result;


namespace App.Services.IServices
{
    public interface IResultService
    {
        CommitSummaryList Summary(ResultsFilter resultsFilter);
        Task<DetailsOfCommitMetrics> CommitDetails(int commitId);
    }
}
