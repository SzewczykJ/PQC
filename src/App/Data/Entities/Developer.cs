using System.ComponentModel.DataAnnotations;

namespace App.Data.Entities
{
    public class Developer
    {
        [Key]
        public int DeveloperId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }
#nullable enable
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
#nullable restore
    }
}
