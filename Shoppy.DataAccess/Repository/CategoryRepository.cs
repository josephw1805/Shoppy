using Shoppy.Models;

namespace Shoppy.DataAccess;

public class CategoryRepository(ApplicationDbContext db) : Repository<Category>(db), ICategoryRepository
{
  private readonly ApplicationDbContext _db = db;

  public void Update(Category obj)
  {
    _db.Categories.Update(obj);
  }
}
