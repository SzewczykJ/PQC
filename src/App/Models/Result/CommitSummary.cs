using System;
using App.Data.Entities;

namespace App.Models.Result
{
    public class CommitSummary
    {
        public int CommitId { get; set; }
        public DateTime Date { get; set; }

        public Developer Developer { get; set; }

        public Branch Branch { get; set; }

        public AverageMetrics Metrics { get; set; }
#nullable enable
        public string? Sha { get; set; }
        public string? Message { get; set; }
#nullable restore
    }
}
