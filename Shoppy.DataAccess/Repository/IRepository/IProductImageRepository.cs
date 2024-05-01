using Shoppy.Models;

namespace Shoppy.DataAccess;

public interface IProductImageRepository : IRepository<ProductImage>
{
  void Update(ProductImage obj);
}
