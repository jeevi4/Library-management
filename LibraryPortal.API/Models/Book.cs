using System.ComponentModel.DataAnnotations;

namespace LibraryPortal.API.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int PublishedYear { get; set; }
        public string AvailabilityStatus { get; set; } = "Available";
        public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
    }
}