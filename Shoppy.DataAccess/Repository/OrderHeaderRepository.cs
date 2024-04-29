using Shoppy.Models;

namespace Shoppy.DataAccess;

public class OrderHeaderRepository(ApplicationDbContext db) : Repository<OrderHeader>(db), IOrderHeaderRepository
{
  private readonly ApplicationDbContext _db = db;
  public void Update(OrderHeader obj)
  {
    _db.OrderHeaders.Update(obj);
  }

  public void UpdateStatus(int id, string orderStatus, string paymentStatus = null)
  {
    var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
    if (orderFromDb != null)
    {
      orderFromDb.OrderStatus = orderStatus;
      if (!string.IsNullOrEmpty(paymentStatus))
      {
        orderFromDb.PaymentStatus = paymentStatus;
      }
    }
  }

  public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
  {
    var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
    if (!string.IsNullOrEmpty(sessionId))
    {
      orderFromDb.SessionId = sessionId;
    }
    if (!string.IsNullOrEmpty(paymentIntentId))
    {
      orderFromDb.PaymentIntentId = paymentIntentId;
      orderFromDb.PaymentDate = DateTime.Now;
    }
  }
}
