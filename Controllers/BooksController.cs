using BookLendAPI.Data;
using BookLendAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookLendAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext _libraryDbContext;

        public BooksController(LibraryDbContext libraryDbContext)
        {
            _libraryDbContext = libraryDbContext;
        }

        [HttpPost("AddSampleDataToDb")]
        public async Task<ActionResult> AddSampleDataToDb()
        {
            if (!_libraryDbContext.Books.Any())
            {
                _libraryDbContext.Books.AddRange(
                    new Book { Title = "A" },
                    new Book { Title = "B" },
                    new Book { Title = "C" },
                    new Book { Title = "D" },
                    new Book { Title = "E" },
                    new Book { Title = "F" },
                    new Book { Title = "G" },
                    new Book { Title = "H" },
                    new Book { Title = "I" },
                    new Book { Title = "J" }
                );
            }

            if (!_libraryDbContext.Users.Any())
            {
                _libraryDbContext.Users.AddRange(
                    new User { Name = "AA" },
                    new User { Name = "BB" },
                    new User { Name = "CC" }
                );
            }

            await _libraryDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("GetAllDataFromDb")]
        public async Task<ActionResult> GetAllDataFromDb()
        {
            var allData = new
            {
                Books = await _libraryDbContext.Books.ToListAsync(),
                Loans = await _libraryDbContext.Loans.ToListAsync(),
                Users = await _libraryDbContext.Users.ToListAsync()
            };

            return Ok(allData);
        }

        [HttpPost("Create")]
        public async Task<ActionResult> CreateBook(Book book)
        {
            _libraryDbContext.Books.Add(book);

            await _libraryDbContext.SaveChangesAsync();

            return Ok(book);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _libraryDbContext.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        [HttpPatch("{id}/{title}")]
        public async Task<IActionResult> UpdateBook(int id, string title)
        {
            var book = await _libraryDbContext.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            book.Title = title;

            _libraryDbContext.Entry(book).State = EntityState.Modified;

            try
            {
                await _libraryDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(book);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _libraryDbContext.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            _libraryDbContext.Books.Remove(book);

            await _libraryDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("LendBook")]
        public async Task<IActionResult> CreateLend(int userId, int bookId)
        {
            var user = await _libraryDbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            //In simple scenario like this we can keep this,
            //but in large scaled app we need to use DB Indexing of 
            //often fetched BookId and DateReturned or adding IsAvailable to book model etc.
            var isBookLoaned = await _libraryDbContext.Loans.AnyAsync(l => l.BookId == bookId && l.DateReturned == null);
            if (isBookLoaned)
            {
                return BadRequest("Book is already loaned.");
            }

            var loan = new Loan
            {
                UserId = userId,
                BookId = bookId,
                DateBorrowed = DateTime.Now
            };

            _libraryDbContext.Loans.Add(loan);

            try
            {
                await _libraryDbContext.SaveChangesAsync();
                return Ok("Book successfully lent.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("return/{lendId}")]
        public async Task<IActionResult> ConfirmReturn(int lendId)
        {
            var loan = await _libraryDbContext.Loans.FindAsync(lendId);

            if (loan == null)
            {
                return NotFound("Lend record not found.");
            }

            if (loan.DateReturned != null)
            {
                return BadRequest("Book has already been returned.");
            }

            loan.DateReturned = DateTime.Now;

            try
            {
                await _libraryDbContext.SaveChangesAsync();
                return Ok("Book return confirmed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool BookExists(int id)
        {
            return _libraryDbContext.Books.Any(e => e.Id == id);
        }

        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //return CreatedAtAction(nameof(CreateBook), new { id = book.Id }, book);
    }
}
