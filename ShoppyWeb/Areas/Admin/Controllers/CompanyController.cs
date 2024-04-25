using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppy.DataAccess;
using Shoppy.Models;
using Shoppy.Utility;

namespace ShoppyWeb;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class CompanyController(IUnitOfWork unitOfWork) : Controller
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public IActionResult Index()
  {
    List<Company> objCompanyList = [.. _unitOfWork.Company.GetAll()];
    return View(objCompanyList);
  }

  public IActionResult Upsert(int? id)
  {

    if (id == null || id == 0)
    {
      // Create
      return View(new Company());
    }
    else
    {
      // Edit
      Company companyFromDb = _unitOfWork.Company.Get(u => u.Id == id);
      return View(companyFromDb);
    }
  }

  [HttpPost]
  public IActionResult Upsert(Company obj)
  {
    if (ModelState.IsValid)
    {
      if (obj.Id == 0)
      {
        _unitOfWork.Company.Add(obj);
      }
      else
      {
        _unitOfWork.Company.Update(obj);
      }

      _unitOfWork.Save();
      TempData["success"] = "Company created successfully";
      return RedirectToAction("Index");
    }
    return View(obj);
  }

  #region API CALLS
  [HttpGet]
  public IActionResult GetAll()
  {
    List<Company> objCompanyList = [.. _unitOfWork.Company.GetAll()];
    return Json(new { data = objCompanyList });
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

