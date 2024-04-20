using Microsoft.EntityFrameworkCore;

namespace ShoppyWeb;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
  public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Category>().HasData(
        new Category {Id=1, Name="Electronic Pop", DisplayOrder= 1},
        new Category {Id=2, Name="Pop", DisplayOrder= 2},
        new Category {Id=3, Name="Electronic, Rock, Pop", DisplayOrder= 3}
      );
    }
}
