using Readify_Library.Models;

namespace Readify_Library.ViewModels
{
    public class UserDetailsViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string BirthDate { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public string TypeOfUser { get; set; }
        public string ProfileImageUrl { get; set; }
        public int TotalBorrowedBooks { get; set; }
        public List<Borrowing>? BorrowedBooks { get; set; }
    }
}
