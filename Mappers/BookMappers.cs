using Exadel.BookManagement.API.DTOs.Book;
using Exadel.BookManagement.API.Models;

namespace Exadel.BookManagement.API.Mappers
{
    public static class BookMappers
    {
        public static Book ToBookFromBookDto(this BookDto bookDto)
        {
            return new Book
            {
                Title = bookDto.Title,
                AuthorName = bookDto.AuthorName,
                PublicationYear = bookDto.PublicationYear
            };
        }
    }
}
