using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Readify_Library.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100, ErrorMessage = "Max Length of First Name Must be 100 Characters at most!.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Max Length of Last Name Must be 100 Characters at most!.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Max Length of Address Must be 100 Characters at most!.")]
        public string Address { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        public bool IsActive { get; set; } = true;

        [MinLength(1)]
        [MaxLength(200)]
        public int NumberOfBooksAvailable { get; set; }


        [ForeignKey(nameof(UserType))]
        [Display(Name = "User Type")]
        public int UserTypeId { get; set; }
        public UserType UserType { get; set; }


        public List<Borrowing> Borrowings { get; set; }
    }
}
