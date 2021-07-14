using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Data.Entities
{
    public class Metric
    {
        [Key]
        public int MetricId { get; set; }

        public int? Complexity { get; set; }
        public int? CognitiveComplexity { get; set; }
        public int? DuplicatedLines { get; set; }
        public int? CodeSmells { get; set; }
        public int? NewCodeSmells { get; set; }
        public int? CommentLines { get; set; }
        public double? CommentLinesDensity { get; set; }
        public int? Ncloc { get; set; }
        public int? Statements { get; set; }
        public double? BranchCoverage { get; set; }
        public double? LineCoverage { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("File")]
        public int FileId { get; set; }

        [Required]
        public File File { get; set; }
    }
}
