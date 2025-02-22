using System.ComponentModel.DataAnnotations;

namespace Exadel.BookManagement.API.DTOs.AuthAccount
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
