using EcommerceBookApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBookApp.DataAccess;
    
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CoverType> CoverTypes { get; set; }

}
