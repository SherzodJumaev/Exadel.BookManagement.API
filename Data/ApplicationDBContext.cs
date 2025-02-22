using Exadel.BookManagement.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace Exadel.BookManagement.API.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {

        }

        public DbSet<Book> Books { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().HasQueryFilter(b => !b.IsDeleted);

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "d1a1c2b3-4567-8910-1112-abcdef123456",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                },

                new IdentityRole
                {
                    Id = "e2b2c3d4-5678-9101-1213-fedcba654321",
                    Name = "User",
                    NormalizedName = "USER",
                }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);

            


            base.OnModelCreating(modelBuilder);
        }
    }
}
