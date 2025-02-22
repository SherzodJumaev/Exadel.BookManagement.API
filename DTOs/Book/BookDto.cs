using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace Exadel.BookManagement.API.DTOs.Book
{
    public class BookDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 100 characters.")]
        public string Title { get; set; } 
        [Required]
        [Range(1000, 2025, ErrorMessage = "Publication year must be a four-digit number and lower than current year ($2025).")]
        public int PublicationYear { get; set; }
        [Required(ErrorMessage = "Author name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Author name must be between 2 and 50 characters.")]
        public string AuthorName { get; set; }
    }
}
