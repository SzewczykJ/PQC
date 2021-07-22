using System.ComponentModel.DataAnnotations;

namespace App.Models.Result
{
    public class ResultsFilter
    {
        [Required]
        public long RepositoryId { get; set; }

        [Required]
        public int BranchId { get; set; }
    }
}
