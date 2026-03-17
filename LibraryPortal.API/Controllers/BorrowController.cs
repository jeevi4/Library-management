using LibraryPortal.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibraryPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BorrowController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BorrowController(AppDbContext context)
        {
            _context = context;
        }

        // Helper — gets the logged-in user's ID from the JWT token
        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // POST api/borrow/5 — borrow a book
        [HttpPost("{bookId}")]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var userId = GetUserId();

            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return NotFound("Book not found.");

            if (book.AvailabilityStatus == "Borrowed")
                return BadRequest("Book is already borrowed.");

            // Check if this user already borrowed this book
            var alreadyBorrowed = await _context.BorrowRecords
                .AnyAsync(b => b.BookId == bookId &&
                               b.UserId == userId &&
                               b.Status == "Borrowed");

            if (alreadyBorrowed)
                return BadRequest("You have already borrowed this book.");

            // Create borrow record
            var record = new Models.BorrowRecord
            {
                BookId = bookId,
                UserId = userId,
                BorrowDate = DateTime.UtcNow,
                Status = "Borrowed"
            };

            // Mark book as borrowed
            book.AvailabilityStatus = "Borrowed";

            _context.BorrowRecords.Add(record);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Book borrowed successfully.",
                borrowId = record.BorrowId,
                borrowDate = record.BorrowDate
            });
        }

        // PUT api/borrow/return/5 — return a book
        [HttpPut("return/{borrowId}")]
        public async Task<IActionResult> ReturnBook(int borrowId)
        {
            var userId = GetUserId();

            var record = await _context.BorrowRecords
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.BorrowId == borrowId &&
                                          b.UserId == userId);

            if (record == null)
                return NotFound("Borrow record not found.");

            if (record.Status == "Returned")
                return BadRequest("Book already returned.");

            // Update record and book status
            record.Status = "Returned";
            record.ReturnDate = DateTime.UtcNow;
            record.Book.AvailabilityStatus = "Available";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Book returned successfully.",
                returnDate = record.ReturnDate
            });
        }

        // GET api/borrow/my-history — user's own borrow history
        [HttpGet("my-history")]
        public async Task<IActionResult> GetMyHistory()
        {
            var userId = GetUserId();

            var records = await _context.BorrowRecords
                .Include(b => b.Book)
                .Where(b => b.UserId == userId)
                .Select(b => new
                {
                    b.BorrowId,
                    b.BorrowDate,
                    b.ReturnDate,
                    b.Status,
                    Book = new
                    {
                        b.Book.BookId,
                        b.Book.Title,
                        b.Book.Author,
                        b.Book.Category
                    }
                })
                .ToListAsync();

            return Ok(records);
        }

        // GET api/borrow/all — Admin only: see all borrow records
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBorrows()
        {
            var records = await _context.BorrowRecords
                .Include(b => b.Book)
                .Include(b => b.User)
                .Select(b => new
                {
                    b.BorrowId,
                    b.BorrowDate,
                    b.ReturnDate,
                    b.Status,
                    Book = new { b.Book.BookId, b.Book.Title, b.Book.Author },
                    User = new { b.User.UserId, b.User.Name, b.User.Email }
                })
                .ToListAsync();

            return Ok(records);
        }

        // GET api/borrow/dashboard — Admin dashboard statistics
        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDashboard()
        {
            var totalBooks = await _context.Books.CountAsync();
            var totalUsers = await _context.Users
                .CountAsync(u => u.Role == "User");
            var borrowedBooks = await _context.Books
                .CountAsync(b => b.AvailabilityStatus == "Borrowed");
            var totalBorrows = await _context.BorrowRecords.CountAsync();

            return Ok(new
            {
                totalBooks,
                totalUsers,
                borrowedBooks,
                availableBooks = totalBooks - borrowedBooks,
                totalBorrows
            });
        }
    }
}