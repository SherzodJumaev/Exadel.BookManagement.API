using Exadel.BookManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Exadel.BookManagement.API.Data
{
    public class ApplicationDBContext : DbContext 
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
            
        }

        public DbSet<Book> Books { get; set; }
    }
}
