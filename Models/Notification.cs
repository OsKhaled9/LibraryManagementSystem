using System.ComponentModel.DataAnnotations;

namespace Readify_Library.Models
{
    public enum NotificationType
    {
        BorrowApproved,
        BorrowRejected,
        BookReturned,
        OverdueWarning,
        System
    }

    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        public bool IsRead { get; set; } = false;

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public NotificationType NotificationType { get; set; }
    }
}
