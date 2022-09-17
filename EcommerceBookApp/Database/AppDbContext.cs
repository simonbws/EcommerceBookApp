using EcommerceBookApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBookApp.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }

    }
}
