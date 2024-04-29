﻿using Shoppy.Models;

namespace Shoppy.DataAccess;

public interface IOrderHeaderRepository : IRepository<OrderHeader>
{
  void Update(OrderHeader obj);
  void UpdateStatus(int id, string orderStatus, string paymentStatus = null);
  void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
}
