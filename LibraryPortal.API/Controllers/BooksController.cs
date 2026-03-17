using LibraryPortal.API.Data;
using LibraryPortal.API.DTOs;
using LibraryPortal.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/books — both Admin and User can view
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _context.Books.ToListAsync();
            return Ok(books);
        }

        // GET api/books/search?query=harry — search by title or author
        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks([FromQuery] string query)
        {
            var books = await _context.Books
                .Where(b => b.Title.Contains(query) ||
                            b.Author.Contains(query))
                .ToListAsync();
            return Ok(books);
        }

        // GET api/books/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound("Book not found.");
            return Ok(book);
        }

        // POST api/books — Admin only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBook(BookDto dto)
        {
            var book = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                Category = dto.Category,
                PublishedYear = dto.PublishedYear,
                AvailabilityStatus = "Available"
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return Ok(book);
        }

        // PUT api/books/5 — Admin only
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook(int id, BookDto dto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound("Book not found.");

            book.Title = dto.Title;
            book.Author = dto.Author;
            book.Category = dto.Category;
            book.PublishedYear = dto.PublishedYear;

            await _context.SaveChangesAsync();
            return Ok(book);
        }

        // DELETE api/books/5 — Admin only
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound("Book not found.");

            // Prevent deleting a currently borrowed book
            var isBorrowed = await _context.BorrowRecords
                .AnyAsync(b => b.BookId == id && b.Status == "Borrowed");

            if (isBorrowed)
                return BadRequest("Cannot delete a book that is currently borrowed.");

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return Ok("Book deleted successfully.");
        }
    }
}
