using Shoppy.Models;

namespace Shoppy.DataAccess;

public interface IOrderHeaderRepository : IRepository<OrderHeader>
{
  void Update(OrderHeader obj);
}
