using LibGit2Sharp;

namespace App.Models
{
    public class CommitChanges
    {
        public ChangeKind Status { get; set; }
        public string SHA { get; set; }
    }
}
