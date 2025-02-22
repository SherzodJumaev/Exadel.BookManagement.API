namespace Exadel.BookManagement.API.DTOs.Book
{
    public class BookMainDto
    {
        public string Title { get; set; }
        public int PublicationYear { get; set; }
        public string AuthorName { get; set; }
        public int BookViews { get; set; } = 0;
        public double PopularityScore { get; set; }
    }
}
