using Shoppy.Models;

namespace Shoppy.DataAccess;

public class ProductRepository(ApplicationDbContext db) : Repository<Product>(db), IProductRepository
{
  private readonly ApplicationDbContext _db = db;
  public void Update(Product obj)
  {
    var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
    if (objFromDb != null)
    {
      objFromDb.Title = obj.Title;
      objFromDb.ISBN = obj.ISBN;
      objFromDb.Price = obj.Price;
      objFromDb.Price50 = obj.Price50;
      objFromDb.ListPrice = obj.ListPrice;
      objFromDb.Price100 = obj.Price100;
      objFromDb.Description = obj.Description;
      objFromDb.CategoryId = obj.CategoryId;
      objFromDb.Author = obj.Author;
      objFromDb.ProductImages = obj.ProductImages;
    }
  }
}
