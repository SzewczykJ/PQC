using System.Collections.Generic;
using System.Linq;
using App.Data.Entities;
using App.Data.IDataRepository;
using App.Models.Result;
using Microsoft.EntityFrameworkCore;

namespace App.Data.DataRepository
{
    public class MetricsRepo : IMetricRepo
    {
        private readonly AppDbContext context;

        public MetricsRepo(AppDbContext context)
        {
            this.context = context;
        }

        public int Add(Metric metric)
        {
            this.context.Metrics.Add(metric);
            return this.context.SaveChanges();
        }

        public int Update(Metric metric)
        {
            this.context.Metrics.Update(metric);
            return this.context.SaveChanges();
        }

        public int Delete(Metric metric)
        {
            this.context.Metrics.Remove(metric);
            return this.context.SaveChanges();
        }

        public Dictionary<int, AverageMetrics> GetAverageMetricsGroupedByCommit(long repositoryId, int? branchId = null)
        {
            var query = this.context.Files
                .Include(metric => metric.Metric)
                .Where(r => r.Commit.Repository.RepositoryId == repositoryId).AsQueryable();

            if (branchId.HasValue)
            {
                query = query.Where(r => r.Commit.Branch.BranchId == branchId);
            }

            query = query.OrderBy(c => c.Commit.Date);
            var selectedMetrics = query.Select(metric => new {Key = metric.Commit.CommitId, Value = metric.Metric})
                .AsEnumerable();
            var groupedMetrics = selectedMetrics.GroupBy(c => c.Key, value => value.Value);

            var response = new Dictionary<int, AverageMetrics>();
            foreach (var group in groupedMetrics)
            {
                response.Add(group.Key, this.CalculateAverageMetrics(group.ToList()));
            }

            return response;
        }

        private AverageMetrics CalculateAverageMetrics(List<Metric> metrics)
        {
            return new AverageMetrics
            {
                BranchCoverage = metrics.Where(t => t.BranchCoverage.HasValue).Average(t => t.BranchCoverage),
                CodeSmells = metrics.Where(t => t.CodeSmells.HasValue).Average(t => t.CodeSmells),
                CognitiveComplexity = metrics.Where(t => t.CognitiveComplexity.HasValue)
                    .Average(t => t.CognitiveComplexity),
                Complexity = metrics.Where(t => t.Complexity.HasValue).Average(t => t.Complexity),
                DuplicatedLines = metrics.Where(t => t.DuplicatedLines.HasValue).Average(t => t.DuplicatedLines),
                NewCodeSmells = metrics.Where(t => t.NewCodeSmells.HasValue).Average(t => t.NewCodeSmells),
                CommentLines = metrics.Where(t => t.CommentLines.HasValue).Average(t => t.CommentLines),
                CommentLinesDensity = metrics.Where(t => t.CommentLinesDensity.HasValue)
                    .Average(t => t.CommentLinesDensity),
                Ncloc = metrics.Where(t => t.Ncloc.HasValue).Average(t => t.Ncloc),
                Statements = metrics.Where(t => t.Statements.HasValue).Average(t => t.Statements),
                LineCoverage = metrics.Where(t => t.LineCoverage.HasValue).Average(t => t.LineCoverage)
            };
        }
    }
}
