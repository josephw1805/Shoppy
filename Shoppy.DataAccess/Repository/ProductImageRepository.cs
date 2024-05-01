using Shoppy.Models;

namespace Shoppy.DataAccess;

public class ProductImageRepository(ApplicationDbContext db) : Repository<ProductImage>(db), IProductImageRepository
{
  private readonly ApplicationDbContext _db = db;

  public void Update(ProductImage obj)
  {
    _db.ProductImages.Update(obj);
  }
}
