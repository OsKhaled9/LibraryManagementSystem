using Readify_Library.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Readify_Library.ViewModels
{
    public class BookFormViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Max Length of Title Must be 100 Characters at most!.")]
        public string Title { get; set; }

        [Required]
        [StringLength(60, ErrorMessage = "Max Length of Author Must be 60 Characters at most!.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Author name must contain only letters.")]
        public string Author { get; set; }

        public string? ISBN { get; set; }

        [Required]
        [Range(1900,2999, ErrorMessage = "Publisher Year Must be Betwwen 1990 - 2999")]
        [Display(Name = "Publish Year")]
        public int PublishYear { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Copies Must be Betwwen 0 - 1000")]
        [Display(Name = "Available Copies")]
        public int AvailableCopies { get; set; }

        public string? Description { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
    }
}
