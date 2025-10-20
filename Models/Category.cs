using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Readify_Library.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "Max Length of Name Must be 40 Characters at most!.")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "Category name must contain only Arabic and English letters.")]
        [Display(Name = "Category Name")]
        [Remote(action: "IsUniqueName", controller: "Categories",
            AdditionalFields = "Id", ErrorMessage = "Category name already exists.")]
        public string Name { get; set; }

        public string? Description { get; set; }


        public List<Book>? Books { get; set; }
    }
}
