using Exadel.BookManagement.API.Data;
using Exadel.BookManagement.API.DTOs.Book;
using Exadel.BookManagement.API.Helpers;
using Exadel.BookManagement.API.Interfaces;
using Exadel.BookManagement.API.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Exadel.BookManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly CheckBooks _checkBooks;
        public BooksController(IBookRepository bookRepository, CheckBooks checkBooks)
        {
            _bookRepository = bookRepository;
            _checkBooks = checkBooks;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks([FromQuery] QueryObject queryObject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var books = await _bookRepository.GetAllAsync(queryObject);
            var onlyTitlesOfBooks = books.Select(b => b.ToOnlyTitleFromBook());

            return Ok(onlyTitlesOfBooks);
        }

        [HttpGet("{title}")]
        public async Task<IActionResult> GetBookDetails([FromRoute] string title)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (title.Length < 2)
            {
                return BadRequest("The Title must be at least 2 characters long.");
            }

            var book = await _bookRepository.GetByTitleAsync(title);

            if (book is null)
            {
                return NotFound($"Book with the given Title: \"{title}\" not found.");
            }

            return Ok(book.ToBookMainDtoFromBook());
        }

        [HttpGet("soft-deleted-books-titles")]
        public async Task<IActionResult> GetDeletedBooks()
        {
            var softDeletedBooks = await _bookRepository.GetDeletedBooks();
            var onlyTitlesOfBooks = softDeletedBooks.Select(b => b.ToOnlyTitleFromBook());

            return Ok(onlyTitlesOfBooks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] BookDto bookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bookModel = bookDto.ToBookFromBookDto();

            var result = await _checkBooks.doesBookExistInDB(bookDto);

            if (result)
            {
                return Conflict($"The book \"{bookModel.Title}\" already exist in the database!" +
                    $"\nEdit its title or enter another book!");
            }

            var resultSoftDeletedBook = await _checkBooks.GetSoftDeletedBook(bookDto.Title);

            if (resultSoftDeletedBook.Any())
            {
                return Conflict($"The book \"{bookModel.Title.ToUpper()}\" was 'SOFT DELETED', but already exist in the database!" +
                    $"\nEnter new book's title or 'RESTORE' it!");
            }

            await _bookRepository.CreateAsync(bookModel);

            return CreatedAtAction(nameof(GetBookDetails), new { title = bookModel.Title }, bookModel.ToBookDtoFromBook());
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulkBooks([FromBody] List<BookDto> booksDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (booksDto == null || !booksDto.Any())
            {
                return BadRequest("Book list cannot be empty.");
            }

            var duplicateInRequest = _checkBooks.DuplicatesInRequest(booksDto);

            if (duplicateInRequest.Any())
            {
                return Conflict($"Duplicate books found in the request: \"{string.Join(", ", duplicateInRequest)}\"" +
                    $"\nYou have to enter the books with different names!");
            }

            var newBooks = await _checkBooks.DoBooksExistInDB(booksDto);

            if (newBooks.Count != booksDto.Count)
            {
                var existingTitles = await _checkBooks.ExistingTitles();

                var existingBooks = booksDto
                    .Where(b => existingTitles.Contains(b.Title.ToLower()))
                    .ToList();

                if (existingBooks.Count > 1)
                {
                    return Conflict($"These \"{string.Join(", ", existingBooks.Select(x => x.Title))}\" books already exist in the database!" +
                        $"\nEdit their title or enter other books!");
                }

                return Conflict($"The book \"{existingBooks.First().Title}\" already exist in the database!" +
                    $"\nEdit its title or enter another book!");
            }

            var resultSoftDeletedBook = await _checkBooks.GetSoftDeletedBooks(booksDto.Select(b => b.Title).ToList());

            if (resultSoftDeletedBook.Any())
            {
                return Conflict($"The book/books \"{string.Join(", ", resultSoftDeletedBook.Select(b => b.Title.ToUpper()))}\" was/were 'SOFT DELETED', but already exist in the database!" +
                    $"\nEnter new books title or 'RESTORE' it/them!");
            }

            var bookModels = newBooks.Select(b => b.ToBookFromBookDto());
            var createdBooks = await _bookRepository.CreateBulkAsync(bookModels);

            return CreatedAtAction(nameof(GetAllBooks), new { }, createdBooks.Select(b => b.ToBookDtoFromBook()));
        }

        [HttpPut("{title}")]
        public async Task<IActionResult> UpdateBook([FromRoute] string title, [FromBody] BookDto bookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _checkBooks.doesBookExistInDB(bookDto);

            if (result)
            {
                return Conflict($"The book \"{bookDto.Title}\" already exist in the database!" +
                    $"\nEdit its title or enter another book!");
            }

            var resultSoftDeletedBook = await _checkBooks.GetSoftDeletedBook(bookDto.Title);
            
            if (resultSoftDeletedBook.Any())
            {
                return Conflict($"The book \"{bookDto.Title.ToUpper()}\" was 'SOFT DELETED', but already exist in the database!" +
                    $"\nEnter new book's title or 'RESTORE' it!");
            }

            if (title.Length < 2)
            {
                return BadRequest("The Title must be at least 2 characters long.");
            }

            var bookModel = bookDto.ToBookFromBookDto();

            var updatedBook = await _bookRepository.UpdateAsync(title, bookModel);

            if (updatedBook is null)
            {
                return NotFound($"Book with given Title: \"{title}\" not found.");
            }

            return Ok(updatedBook.ToBookDtoFromBook());
        }

        [HttpPut("restore/{title}")]
        public async Task<IActionResult> RestoreBook([FromRoute] string title)
        {
            var book = await _bookRepository.RestoreBook(title);

            if (book is null)
            {
                return NotFound("Book not found or not deleted.");
            }

            return Ok($"Book '{book.Title}' has been restored successfully.");
        }

        [HttpDelete("{title}")]
        public async Task<IActionResult> SelfDeleteBook([FromRoute] string title)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (title.Length < 2)
            {
                return BadRequest("The Title must be at least 2 characters long.");
            }

            var deletedBookResult = await _bookRepository.DeleteAsync(title);

            if (!deletedBookResult)
            {
                return NotFound($"Book with given Title: \"{title}\" not found.");
            }

            return NoContent();
        }

        [HttpDelete("bulk")]
        public async Task<IActionResult> SelfDeleteBooks([FromQuery] List<string> titles)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool allZero = titles.All(title => title.Length == 0);

            if (titles.Count <= 0)
            {
                return BadRequest("Went something wrong!");
            }

            if (allZero)
            {
                return BadRequest("All titles are empty.");
            }

            var duplicateTitles = titles.GroupBy(b => b)
                .Where(b => b.Count() > 1)
                .Select(b => b.Key)
                .ToList();

            if (duplicateTitles.Any())
            {
                return BadRequest($"Duplicate Titles found: {string.Join(", ", duplicateTitles)}");
            }

            var existingBooks = await _checkBooks.ReturnExistingBooks(titles);

            var deletedBooksResult = await _bookRepository.DeleteBulkAsync(titles);

            var nonExistingTitles = titles.Except(existingBooks.Select(eb => eb.Title)).ToList();

            if (!deletedBooksResult)
            {
                return NotFound($"Books with given Titles: \"{string.Join(", ", titles)}\" are not found.");
            }

            return Ok(new DeleteBulkResponseModel
            {
                DeletedIDs = existingBooks.Any() ? $"Deleted books Titles: \"{string.Join(", ", existingBooks.Select(b => b.Title))}\"" : "Deleted books Titles: None",
                NonExistIDs = nonExistingTitles.Any() ? $"Not found Titles: \"{string.Join(", ", nonExistingTitles)}\"" : "Not found Titles: None"
            });
        }
    }
}
