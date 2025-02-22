using Exadel.BookManagement.API.Data;
using Exadel.BookManagement.API.DTOs.Book;
using Exadel.BookManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Exadel.BookManagement.API.Helpers
{
    public class CheckBooks
    {
        private readonly ApplicationDBContext _context;
        public CheckBooks(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<string>> ExistingTitles()
        {
            var existingTitles = await _context.Books
                .Select(b => b.Title.ToLower())
                .ToListAsync();

            return existingTitles;
        }

        public async Task<bool> doesBookExistInDB(BookDto bookDto)
        {
            var existingTitles = await ExistingTitles();

            var isDuplicate = existingTitles.Contains(bookDto.Title.ToLower());

            return isDuplicate;
        }

        public List<string> DuplicatesInRequest(List<BookDto> bookDtos)
        {
            var duplicateInRequest = bookDtos
                .GroupBy(b => b.Title.ToLower())
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            return duplicateInRequest;
        }

        public async Task<List<BookDto>> DoBooksExistInDB(List<BookDto> bookDtos)
        {
            var existingTitles = await ExistingTitles();

            var newBooks = bookDtos
                .Where(b => !existingTitles.Contains(b.Title.ToLower()))
                .ToList();

            return newBooks;
        }

        public async Task<IEnumerable<Book>> GetTopMaxBooks(int count)
        {
            return await _context.Books
                .OrderByDescending(b => b.Id)
                .Take(count)
                .Select(b => b)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> ReturnExistingBooks(List<string> titles)
        {
            var existingBooks = await _context.Books
                .Where(b => titles.Contains(b.Title))
                .Select(b => b)
                .ToListAsync();

            return existingBooks;
        }

        public async Task<IEnumerable<Book>> GetSoftDeletedBook(string title)
        {
            var softDeletedBook = await _context.Books
                .IgnoreQueryFilters()
                .Where(b => b.IsDeleted && b.Title == title)
                .ToListAsync();

            return softDeletedBook;
        }

        public async Task<IEnumerable<Book>> GetSoftDeletedBooks(List<string> titles)
        {
            var softDeletedBooks = await _context.Books
                .IgnoreQueryFilters()
                .Where(b => b.IsDeleted && titles.Contains(b.Title))
                .ToListAsync();

            return softDeletedBooks;
        }
    }
}
