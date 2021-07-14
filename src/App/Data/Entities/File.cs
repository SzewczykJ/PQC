using System.ComponentModel.DataAnnotations;

namespace App.Data.Entities
{
    public class File
    {
        [Key]
        public int FileId { get; set; }

        public string SHA { get; set; }
        public FileDetail FileDetail { get; set; }

        [Required]
        public Commit Commit { get; set; }

        public Metric Metric { get; set; }
        public string Status { get; set; }
    }
}
