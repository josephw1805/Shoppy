using Shoppy.Models;

namespace Shoppy.DataAccess;

public class OrderHeaderRepository(ApplicationDbContext db) : Repository<OrderHeader>(db), IOrderHeaderRepository
{
  private readonly ApplicationDbContext _db = db;
  public void Update(OrderHeader obj)
  {
    _db.OrderHeaders.Update(obj);
  }
}
