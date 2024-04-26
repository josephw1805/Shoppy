using Shoppy.Models;

namespace Shoppy.DataAccess;

public interface IShoppingCartRepository : IRepository<ShoppingCart>
{
  void Update(ShoppingCart obj);
}
