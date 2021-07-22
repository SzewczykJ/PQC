using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class RepositoryForm
    {
        [Required]
        [StringLength(250, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]
        public string Name { get; set; }


        [Required]
        [Url]
        public string Url { get; set; }

        [Required]
        public string Branch { get; set; }
    }
}
