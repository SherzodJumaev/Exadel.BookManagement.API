using Exadel.BookManagement.API.Models;

namespace Exadel.BookManagement.API.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(AppUser user);
    }
}
