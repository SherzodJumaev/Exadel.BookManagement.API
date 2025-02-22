using Exadel.BookManagement.API.Data;
using Exadel.BookManagement.API.Helpers;
using Exadel.BookManagement.API.Interfaces;
using Exadel.BookManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exadel.BookManagement.API.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly CheckBooks _checkBooks;
        public BookRepository(ApplicationDBContext context, CheckBooks checkBooks)
        {
            _context = context;
            _checkBooks = checkBooks;
        }

        public async Task<List<Book>> GetAllAsync(QueryObject queryObject)
        {
            var books = _context.Books
                .OrderByDescending(b => b.BookViews)
                .Select(b => b)
                .AsQueryable();

            if (!string.IsNullOrEmpty(queryObject.Title))
            {
                books = books.Where(b => b.Title.Contains(queryObject.Title));
            }

            if (queryObject.SortBy.Equals(EnumHelper.Title))
            {
                books = queryObject.IsDescending ? books.OrderByDescending(b => b.Title) : books.OrderBy(b => b.Title);
            }

            var skipNumber = (queryObject.PageNumber - 1) * queryObject.PageSize;

            return await books.Skip(skipNumber).Take(queryObject.PageSize).ToListAsync();
        }

        public async Task<Book> GetByTitleAsync(string title)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Title == title);

            if (book is null)
            {
                return null;
            }


            await _context.Books
                   .Where(b => b.Title == title)
                   .ExecuteUpdateAsync(setters => setters.SetProperty(b => b.BookViews, b => b.BookViews + 1));

            return book;
        }

        public async Task<Book> CreateAsync(Book bookModel)
        {
            await _context.Books.AddAsync(bookModel);
            await _context.SaveChangesAsync();

            return bookModel;
        }

        public async Task<IEnumerable<Book>> CreateBulkAsync(IEnumerable<Book> bookModels)
        {
            await _context.Books.AddRangeAsync(bookModels);
            var count = await _context.SaveChangesAsync();

            var savedBooks = await _checkBooks.GetTopMaxBooks(count);

            return savedBooks.OrderBy(sb => sb.Id);
        }

        public async Task<Book> UpdateAsync(string title, Book bookModel)
        {
            var existingBook = await _context.Books.FirstOrDefaultAsync(b => b.Title == title);

            if (existingBook is null)
            {
                return null;
            }

            existingBook.Title = bookModel.Title;
            existingBook.AuthorName = bookModel.AuthorName;
            existingBook.PublicationYear = bookModel.PublicationYear;

            await _context.SaveChangesAsync();

            return existingBook;
        }

        public async Task<bool> DeleteAsync(string title)
        {
            var existingBook = await _context.Books.FirstOrDefaultAsync(b => b.Title == title);

            if (existingBook is null)
            {
                return false;
            }

            existingBook.IsDeleted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteBulkAsync(List<string> titles)
        {
            var existingBooks = await _checkBooks.ReturnExistingBooks(titles);

            if (existingBooks is null)
            {
                return false;
            }

            foreach (var book in existingBooks)
            {
                book.IsDeleted = true;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Book>> GetDeletedBooks()
        {
            var softDeletedBooks = await _context.Books
                .IgnoreQueryFilters()
                .Where(b => b.IsDeleted)
                .ToListAsync();

            return softDeletedBooks;
        }

        public async Task<Book> RestoreBook([FromRoute] string title)
        {
            var book = await _context.Books
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(b => b.Title == title && b.IsDeleted);

            if (book == null)
            {
                return null;
            }

            book.IsDeleted = false;
            book.BookViews = 0;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return book;
        }
    }
}
