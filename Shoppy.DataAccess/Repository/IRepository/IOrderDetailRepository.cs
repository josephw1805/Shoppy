using Shoppy.Models;

namespace Shoppy.DataAccess;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
  void Update(OrderDetail obj);
}
