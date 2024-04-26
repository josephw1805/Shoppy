using Shoppy.Models;

namespace Shoppy.DataAccess;

public class ShoppingCartRepository(ApplicationDbContext db) : Repository<ShoppingCart>(db), IShoppingCartRepository
{
  private readonly ApplicationDbContext _db = db;
  public void Update(ShoppingCart obj)
  {
    _db.ShoppingCarts.Update(obj);
  }
}