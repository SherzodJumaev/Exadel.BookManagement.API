using Exadel.BookManagement.API.DTOs.Book;
using Exadel.BookManagement.API.Helpers;
using Exadel.BookManagement.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.BookManagement.API.Interfaces
{
    public interface IBookRepository
    {
        Task<List<Book>> GetAllAsync(QueryObject queryObject);
        Task<Book> GetByTitleAsync(string title);
        Task<Book> CreateAsync(Book bookModel);
        Task<IEnumerable<Book>> CreateBulkAsync(IEnumerable<Book> bookModels);
        Task<Book> UpdateAsync(string title, Book bookModel);
        Task<bool> DeleteAsync(string title);
        Task<bool> DeleteBulkAsync(List<string> titles);
        Task<IEnumerable<Book>> GetDeletedBooks();
        Task<Book> RestoreBook([FromRoute] string title);
    }
}
