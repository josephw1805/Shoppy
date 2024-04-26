using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppy.DataAccess;
using Shoppy.Models;

namespace ShoppyWeb;

[Area("customer")]
[Authorize]
public class CartController(IUnitOfWork unitOfWork) : Controller
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
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
