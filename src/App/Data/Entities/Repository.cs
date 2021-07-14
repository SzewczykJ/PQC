using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Data.Entities
{
    public class Repository
    {
        [Key]
        public long RepositoryId { get; set; }

        [Required]
        public string Name { get; set; }

        public string FullName { get; set; }
        public string Url { get; set; }
        public bool Private { get; set; }
        public string Key { get; set; }
        public List<Commit> Commits { get; set; }
    }
}
