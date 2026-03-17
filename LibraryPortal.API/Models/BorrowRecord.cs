using System.ComponentModel.DataAnnotations;

namespace LibraryPortal.API.Models
{
    public class BorrowRecord
    {
        [Key]
        public int BorrowId { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = "Borrowed";

        public Book Book { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}