using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Readify_Library.Models
{
    public enum enBorrowStatus : byte
    {
        Pending = 1,
        OverDue = 2,
        Returned = 3
    }

    public class Borrowing
    {
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime BorrowDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DueDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ReturnDate { get; set; }

        [Range(1, 1000, ErrorMessage = "Penalty Amount Must be Betwwen 1 - 1000")]
        public decimal PenaltyAmount { get; set; }

        public enBorrowStatus Status { get; set; }


        [ForeignKey(nameof(Book))]
        [Display(Name = "Book")]
        public int BookId { get; set; }
        public Book Book { get; set; }


        [ForeignKey(nameof(User))]
        [Display(Name = "User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}

// Id → رقم الاستعارة
// UserId / User → المستخدم الي استعار
// BookId / Book → الكتاب الي استعاره
// BorrowDate → تاريخ الاستعارة
// DueDate → آخر موعد لإرجاع الكتاب
// ReturnDate → تاريخ الإرجاع الفعلي
// PenaltyAmount → الغرامة في حالة تأخير إرجاع الكتاب
// Status → حالة الاستعارة
