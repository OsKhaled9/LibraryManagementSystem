using Readify_Library.Models;

namespace Readify_Library.ViewModels
{
    public class AllUserBorrowingsViewModel
    {
        public int Id { get; set; }
        public string BookTitle { get; set; }
        public string BookAuthor { get; set; }
        public string BookImage { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal PenaltyAmount { get; set; }
        public string Status { get; set; }
    }
}
