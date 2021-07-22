using App.Data.Entities;

namespace App.Models
{
    public class FileDetailsDtos
    {
        public int FileId { get; set; }
        public string SHA { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
#nullable enable
        public string? FullPath { get; set; }
#nullable restore
        public string Language { get; set; }
        public Metric Metric { get; set; }
    }
}
