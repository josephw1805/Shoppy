using Shoppy.Models;

namespace Shoppy.DataAccess;

public class OrderDetailRepository(ApplicationDbContext db) : Repository<OrderDetail>(db), IOrderDetailRepository
{
  private readonly ApplicationDbContext _db = db;
  public void Update(OrderDetail obj)
  {
    _db.OrderDetails.Update(obj);
  }
}
