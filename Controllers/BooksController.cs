using Exadel.BookManagement.API.Data;
using Exadel.BookManagement.API.DTOs.Book;
using Exadel.BookManagement.API.Mappers;
using Exadel.BookManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Exadel.BookManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public BooksController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/<BooksController>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _context.Books.ToListAsync();

            return Ok(books);
        }

        // GET api/<BooksController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var book = await _context.Books.FindAsync(id);

            if (book is null)
            {
                return NotFound($"Book with the given id:{id} is not found.");
            }

            return Ok(book);
        }

        // POST api/<BooksController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BookDto bookDto)
        {
            var bookModel = bookDto.ToBookFromBookDto();

            var existingTitles = await _context.Books
                .Select(b => b.Title.ToLower())
                .ToListAsync();

            var newBook = existingTitles.Contains(bookDto.Title.ToLower());

            if (newBook)
            {
                return Conflict($"This book already exist in the database: \"{bookModel.Title}\"!" +
                    $"\nEdit its title or enter another book!");
            }

            await _context.Books.AddAsync(bookModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = bookModel.Id }, bookModel);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulkBooks([FromBody] List<BookDto> booksDto)
        {
            if (booksDto == null || !booksDto.Any())
            {
                return BadRequest("Book list cannot be empty.");
            }

            var duplicateInRequest = booksDto
                .GroupBy(b => b.Title.ToLower())
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateInRequest.Any())
            {
                return Conflict($"Duplicate books found in the request: \"{string.Join(", ", duplicateInRequest)}\"" +
                    $"\nYou have to enter the books with different names!");
            }

            var existingTitles = await _context.Books
                .Select(b => b.Title.ToLower())
                .ToListAsync();

            var newBooks = booksDto
                .Where(b => !existingTitles.Contains(b.Title.ToLower()))
                .ToList();

            if (newBooks.Count != booksDto.Count)
            {
                var existingBooks = booksDto
                    .Where(b => existingTitles.Contains(b.Title.ToLower()))
                    .ToList();

                if (existingBooks.Count > 1)
                {
                    return Conflict($"These books already exist in the database: \"{string.Join(", ", existingBooks.Select(b => b.Title))}\"!" +
                        $"\nEdit their title or enter other books!");
                }

                return Conflict($"This book already exist in the database: \"{existingBooks.Select(b => b.Title)}\"!" +
                    $"\nEdit its title or enter another book!");
            }

            await _context.Books.AddRangeAsync(newBooks.Select(b => b.ToBookFromBookDto()));
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { }, newBooks);
        }

        // PUT api/<BooksController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BooksController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
