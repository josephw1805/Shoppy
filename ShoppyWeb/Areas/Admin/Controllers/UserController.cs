using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppy.DataAccess;
using Shoppy.Models;
using Shoppy.Utility;

namespace ShoppyWeb;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class UserController(IUnitOfWork unitOfWork) : Controller
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public IActionResult Index()
  {
    return View();
  }

  #region API CALLS
  [HttpGet]
  public IActionResult GetAll()
  {
    List<ApplicationUser> objUserList = [.. _unitOfWork.ApplicationUser.GetAll()];
    return Json(new { data = objUserList });
  }

  [HttpDelete]
  public IActionResult Delete(int id)
  {
    var companyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
    if (companyToBeDeleted == null)
    {
      return Json(new { success = false, message = "Error while deleting" });
    }

    _unitOfWork.Company.Remove(companyToBeDeleted);
    _unitOfWork.Save();

    return Json(new { success = true, message = "Delete successful" });
  }
  #endregion
}

