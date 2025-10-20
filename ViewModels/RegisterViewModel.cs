using Readify_Library.Attributes;
using Readify_Library.Models;
using Readify_Library.Settings;
using System.ComponentModel.DataAnnotations;

namespace Readify_Library.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Max Length of First Name Must be 100 Characters at most!.")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "First name must contain only Arabic and English letters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Max Length of Last Name Must be 100 Characters at most!.")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "Last name must contain only Arabic and English letters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Max Length of Address Must be 100 Characters at most!.")]
        public string Address { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Profile Image")]
        [AllowedExtensions(FileSettings.AllowedExtensions),
        MaxFileSize(FileSettings.MaxFileSizeInBytes)]
        public IFormFile ProfileImage { get; set; } = default!;

        //[Display(Name = "User Type")]
        //public int UserTypeId { get; set; }
        //public IEnumerable<UserType>? UsersTypes { get; set; }
    }
}
