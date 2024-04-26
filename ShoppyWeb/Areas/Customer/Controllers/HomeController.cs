using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppy.DataAccess;
using Shoppy.Models;

namespace ShoppyWeb.Controllers;

[Area("Customer")]
public class HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IActionResult Index()
    {
        IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
        return View(productList);
    }

    public IActionResult Details(int itemid)
    {
        ShoppingCart cart = new()
        {
            Product = _unitOfWork.Product.Get(u => u.Id == itemid, includeProperties: "Category"),
            Count = 1,
            ProductId = itemid
        };

        return View(cart);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        shoppingCart.ApplicationUserId = userId;
        ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);
        if (cartFromDb != null)
        {
            // shopping cart exists
            cartFromDb.Count += shoppingCart.Count;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
        }
        else
        {
            // add cart record
            _unitOfWork.ShoppingCart.Add(shoppingCart);
        }
        TempData["success"] = "Cart updated successfully";
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
