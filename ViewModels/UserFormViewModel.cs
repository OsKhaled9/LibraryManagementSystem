using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Readify_Library.Models;
using System.ComponentModel.DataAnnotations;

namespace Readify_Library.ViewModels
{
    public class UserFormViewModel
    {
        public string? UserId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Max Length of First Name Must be 100 Characters at most!.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name must contain only letters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Max Length of Last Name Must be 100 Characters at most!.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name must contain only letters.")]
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

        //[Display(Name = "Number of Books Available")]
        //[Range(1, 100, ErrorMessage = "Number of Books Available Must be Betwwen 1 - 100")]
        public int NumberOfBooksAvailable { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; } = true;


        [Display(Name = "User Type")]
        public int UserTypeId { get; set; }
        public string? UserType { get; set; }
        public IEnumerable<UserType>? UsersTypes { get; set; }


        [Display(Name = "Role")]
        public string RoleId { get; set; }
        public string? RoleName { get; set; }
        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
}
