using Shoppy.Models;

namespace Shoppy.DataAccess;

public interface IProductRepository : IRepository<Product>
{
  void Update(Product obj);
}
