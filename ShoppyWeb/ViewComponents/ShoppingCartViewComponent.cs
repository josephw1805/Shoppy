using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Shoppy.DataAccess;
using Shoppy.Utility;

namespace ShoppyWeb;

public class ShoppingCartViewComponent(IUnitOfWork unitOfWork) : ViewComponent
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public IViewComponentResult Invoke()
  {
    var ClaimsIdentity = (ClaimsIdentity)User.Identity;
    var claim = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
    if (claim != null)
    {
      if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
      {
        HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
      }

      return View(HttpContext.Session.GetInt32(SD.SessionCart));
    }
    else
    {
      HttpContext.Session.Clear();
      return View(0);
    }
  }
}
