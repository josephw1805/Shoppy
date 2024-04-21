using Shoppy.Models;

namespace Shoppy.DataAccess;

public interface ICategoryRepository : IRepository<Category>
{
  void Update(Category obj);
}
