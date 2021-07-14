using System.Collections.Generic;
using App.Data.Entities;
using App.Models.Result;


namespace App.Data.IDataRepository
{
    public interface IMetricRepo
    {
        int Add(Metric metric);

        int Update(Metric metric);

        int Delete(Metric metric);

        Dictionary<int, AverageMetrics> GetAverageMetricsGroupedByCommit(long repositoryId, int? branchId = null);
    }
}
