using System.Collections.Generic;
using System.Threading.Tasks;
using App.Data.IDataRepository;
using App.Models;
using App.Models.Result;
using App.Services.IServices;

namespace App.Services
{
    public class ResultService : IResultService
    {
        private readonly ICommitRepo commitRepository;
        private readonly IFileRepo fileRepository;
        private readonly IMetricRepo metricRepository;

        public ResultService(ICommitRepo commitRepo, IMetricRepo metricsRepo, IFileRepo fileRepo)
        {
            this.commitRepository = commitRepo;
            this.metricRepository = metricsRepo;
            this.fileRepository = fileRepo;
        }

        public CommitSummaryList Summary(ResultsFilter resultsFilter)
        {
            var commitSummaryList =
                this.commitRepository.GetCommitSummaries(resultsFilter.RepositoryId, resultsFilter.BranchId);
            var groupedMetrics =
                this.metricRepository.GetAverageMetricsGroupedByCommit(resultsFilter.RepositoryId,
                    resultsFilter.BranchId);

            foreach (var commit in commitSummaryList.CommitList)
            {
                if (!groupedMetrics.ContainsKey(commit.CommitId))
                {
                    continue;
                }

                commit.Metrics = groupedMetrics.GetValueOrDefault(commit.CommitId);
            }

            return commitSummaryList;
        }

        public async Task<DetailsOfCommitMetrics> CommitDetails(int commitId)
        {
            var result = new DetailsOfCommitMetrics();
            result.CommitId = commitId;
            result.FileList = await this.fileRepository.GetListOfFilesWithDetails(commitId);
            return result;
        }
    }
}
