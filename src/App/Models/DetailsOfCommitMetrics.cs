using System.Collections.Generic;

namespace App.Models
{
    public class DetailsOfCommitMetrics
    {
        public int CommitId { get; set; }
        public IEnumerable<FileDetailsDtos> FileList { get; set; }
    }
}
