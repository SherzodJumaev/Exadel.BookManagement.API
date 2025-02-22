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

        public static BookDtoForTitle ToOnlyTitleFromBook(this Book book)
        {
            return new BookDtoForTitle
            {
                Title = book.Title
            };
        }

        public static BookMainDto ToBookMainDtoFromBook(this Book bookModel)
        {
            return new BookMainDto
            {
                Title = bookModel.Title,
                AuthorName = bookModel.AuthorName,
                BookViews = bookModel.BookViews,
                PublicationYear = bookModel.PublicationYear,
                PopularityScore = bookModel.BookViews * 0.5 + ((DateTime.Now.Year - bookModel.PublicationYear) * 2)
            };
        }

        public static BookDto ToBookDtoFromBook(this Book bookModel)
        {
            return new BookDto
            {
                Title = bookModel.Title,
                AuthorName = bookModel.AuthorName,
                PublicationYear = bookModel.PublicationYear
            };
        }
    }
}
