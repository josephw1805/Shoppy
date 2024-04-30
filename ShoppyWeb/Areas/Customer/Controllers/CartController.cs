﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppy.DataAccess;
using Shoppy.Models;
using Shoppy.Utility;
using Stripe.Checkout;

namespace ShoppyWeb;

[Area("customer")]
[Authorize]
public class CartController(IUnitOfWork unitOfWork) : Controller
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  [BindProperty]
  public ShoppingCartVM ShoppingCartVM { get; set; }

  public IActionResult Index()
  {
    var claimsIdentity = (ClaimsIdentity)User.Identity;
    var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
    ShoppingCartVM = new()
    {
      ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
      OrderHeader = new()
    };

    foreach (var cart in ShoppingCartVM.ShoppingCartList)
    {
      cart.Price = GetPriceBasedOnQuantity(cart);
      ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
    }

    return View(ShoppingCartVM);
  }

  public IActionResult Summary()
  {
    var claimsIdentity = (ClaimsIdentity)User.Identity;
    var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
    ShoppingCartVM = new()
    {
      ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
      OrderHeader = new()
    };

    ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
    ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
    ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
    ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
    ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
    ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
    ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

    foreach (var cart in ShoppingCartVM.ShoppingCartList)
    {
      cart.Price = GetPriceBasedOnQuantity(cart);
      ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
    }

    return View(ShoppingCartVM);
  }

  [HttpPost]
  [ActionName("Summary")]
  public IActionResult SummaryPOST()
  {
    var claimsIdentity = (ClaimsIdentity)User.Identity;
    var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
    ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");

    ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
    ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

    ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

    foreach (var cart in ShoppingCartVM.ShoppingCartList)
    {
      cart.Price = GetPriceBasedOnQuantity(cart);
      ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
    }

    if (applicationUser.CompanyId.GetValueOrDefault() == 0)
    {
      // It is a regular customer
      ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
      ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
    }
    else
    {
      // it is a company user
      ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
      ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
    }
    _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
    _unitOfWork.Save();

    foreach (var cart in ShoppingCartVM.ShoppingCartList)
    {
      OrderDetail orderDetail = new()
      {
        ProductId = cart.ProductId,
        OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
        Price = cart.Price,
        Count = cart.Count
      };
      _unitOfWork.OrderDetail.Add(orderDetail);
      _unitOfWork.Save();
    }

    if (applicationUser.CompanyId.GetValueOrDefault() == 0)
    {
      // stripe logic
      var domain = Request.Scheme + "://" + Request.Host.Value + "/";
      var options = new SessionCreateOptions
      {
        SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
        CancelUrl = domain + "customer/cart/index",
        LineItems = [],
        Mode = "payment"
      };

      foreach (var item in ShoppingCartVM.ShoppingCartList)
      {
        var sessionLineItem = new SessionLineItemOptions
        {
          PriceData = new SessionLineItemPriceDataOptions
          {
            UnitAmount = (long)(item.Price * 100),
            Currency = "usd",
            ProductData = new SessionLineItemPriceDataProductDataOptions
            {
              Name = item.Product.Title
            }
          },
          Quantity = item.Count
        };
        options.LineItems.Add(sessionLineItem);
      }

      var service = new SessionService();
      Session session = service.Create(options);
      _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
      _unitOfWork.Save();
      Response.Headers.Append("Location", session.Url);
      return new StatusCodeResult(303);
    }

    return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
  }

  public IActionResult OrderConfirmation(int id)
  {
    OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
    if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
    {
      // this is an order by customer
      var service = new SessionService();
      Session session = service.Get(orderHeader.SessionId);

      if (session.PaymentStatus.Equals("paid", StringComparison.CurrentCultureIgnoreCase))
      {
        _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
        _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
        _unitOfWork.Save();
      }
      HttpContext.Session.Clear();
    }

    List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
    _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
    _unitOfWork.Save();
    return View(id);
  }

  public IActionResult Plus(int itemid)
  {
    var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == itemid);
    cartFromDb.Count++;
    _unitOfWork.ShoppingCart.Update(cartFromDb);
    _unitOfWork.Save();
    return RedirectToAction(nameof(Index));
  }

  public IActionResult Minus(int itemid)
  {
    var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == itemid);
    if (cartFromDb.Count <= 1)
    {
      // remove from cart
      _unitOfWork.ShoppingCart.Remove(cartFromDb);
      HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
    }
    else
    {
      cartFromDb.Count--;
      _unitOfWork.ShoppingCart.Update(cartFromDb);
    }
    _unitOfWork.Save();
    return RedirectToAction(nameof(Index));
  }

  public IActionResult Remove(int itemid)
  {
    var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == itemid);
    _unitOfWork.ShoppingCart.Remove(cartFromDb);
    HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
    _unitOfWork.Save();
    return RedirectToAction(nameof(Index));
  }

  private static double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
  {
    if (shoppingCart.Count <= 50)
    {
      return shoppingCart.Product.Price;
    }
    else if (shoppingCart.Count <= 100)
    {
      return shoppingCart.Product.Price50;
    }
    else
    {
      return shoppingCart.Product.Price100;
    }
  }
}
