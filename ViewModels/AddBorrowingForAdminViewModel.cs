using Readify_Library.Models;
using System.ComponentModel.DataAnnotations;

namespace Readify_Library.ViewModels
{
    public class AddBorrowingForAdminViewModel
    {
        [Required(ErrorMessage = "Please select a book")]
        [Display(Name = "Book")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Please select a user")]
        [Display(Name = "User")]
        public string UserId { get; set; }

        public IEnumerable<Book>? AvailableBooks { get; set; }
        public IEnumerable<ApplicationUser>? AvailableUsers { get; set; }
    }
}
