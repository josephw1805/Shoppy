using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shoppy.DataAccess;
using Shoppy.Models;
using Shoppy.Utility;

namespace ShoppyWeb;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager) : Controller
{
  private readonly ApplicationDbContext _db = db;
  private readonly UserManager<IdentityUser> _userManager = userManager;

  public IActionResult Index()
  {
    return View();
  }

  #region API CALLS
  [HttpGet]
  public IActionResult GetAll()
  {
    List<ApplicationUser> objUserList = [.. _db.ApplicationUsers.Include(u => u.Company)];

    var userRoles = _db.UserRoles.ToList();
    var roles = _db.Roles.ToList();

    foreach (var user in objUserList)
    {
      var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
      user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
    }

    return Json(new { data = objUserList });
  }

  public IActionResult RoleManagement(string userId)
  {
    string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;
    RoleManagementVM RoleVM = new()
    {
      ApplicationUser = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
      RoleList = _db.Roles.Select(i => new SelectListItem
      {
        Text = i.Name,
        Value = i.Name
      }),
      CompanyList = _db.Companies.Select(i => new SelectListItem
      {
        Text = i.Name,
        Value = i.Id.ToString()
      })
    };
    RoleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;
    return View(RoleVM);
  }

  [HttpPost]
  public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
  {
    string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagementVM.ApplicationUser.Id).RoleId;
    string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;

    if (roleManagementVM.ApplicationUser.Role != oldRole)
    {
      // a role was updated
      ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == roleManagementVM.ApplicationUser.Id);
      if (roleManagementVM.ApplicationUser.Role == SD.Role_Company)
      {
        applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
      }
      if (oldRole == SD.Role_Company)
      {
        applicationUser.CompanyId = null;
      }
      _db.SaveChanges();
      _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
      _userManager.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();
    }
    return RedirectToAction("Index");
  }

  [HttpPost]
  public IActionResult LockUnlock([FromBody] string id)
  {
    var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
    if (objFromDb == null)
    {
      return Json(new { success = false, message = "Error while Locking/Unlocking" });
    }

    if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
    {
      // user is currently locked and we need to unlock them
      objFromDb.LockoutEnd = DateTime.Now;
    }
    else
    {
      objFromDb.LockoutEnd = DateTime.Now.AddYears(100);
    }
    _db.SaveChanges();
    return Json(new { success = true, message = "Operation Successful" });
  }
  #endregion
}

