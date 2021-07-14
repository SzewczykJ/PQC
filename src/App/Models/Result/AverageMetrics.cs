namespace App.Models.Result
{
    public class AverageMetrics
    {
        public double? Complexity { get; set; }
        public double? CognitiveComplexity { get; set; }
        public double? DuplicatedLines { get; set; }
        public double? CodeSmells { get; set; }
        public double? NewCodeSmells { get; set; }
        public double? CommentLines { get; set; }
        public double? CommentLinesDensity { get; set; }
        public double? Ncloc { get; set; }
        public double? Statements { get; set; }
        public double? BranchCoverage { get; set; }
        public double? LineCoverage { get; set; }
    }
}
