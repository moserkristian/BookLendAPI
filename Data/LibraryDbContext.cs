using BookLendAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLendAPI.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {

        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Loan> Loans => Set<Loan>();
        public DbSet<User> Users => Set<User>(); 

    }
}
