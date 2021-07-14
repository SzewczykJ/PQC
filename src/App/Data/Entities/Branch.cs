using System.Collections.Generic;

namespace App.Data.Entities
{
    public class Branch
    {
        public int BranchId { get; set; }
        public string Name { get; set; }

        public List<Commit> Commits { get; set; }
    }
}
