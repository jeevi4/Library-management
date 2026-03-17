using System.ComponentModel.DataAnnotations;

namespace LibraryPortal.API.DTOs
{
    public class BookDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public int PublishedYear { get; set; }
    }
}