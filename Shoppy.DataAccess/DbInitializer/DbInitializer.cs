﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shoppy.Models;
using Shoppy.Utility;

namespace Shoppy.DataAccess;

public class DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db) : IDbInitializer
{
  private readonly UserManager<IdentityUser> _userManager = userManager;
  private readonly RoleManager<IdentityRole> _roleManager = roleManager;
  private readonly ApplicationDbContext _db = db;

  public void Initialize()
  {
    // migrations if they are not applied
    try
    {
      if (_db.Database.GetPendingMigrations().Any())
      {
        _db.Database.Migrate();
      }
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }

    // create roles if they are not created
    if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
    {
      _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
      _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
      _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
      _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

      // if roles are not created, then create admin user
      _userManager.CreateAsync(new ApplicationUser
      {
        UserName = "admin@gmail.com",
        Email = "admin@gmail.com",
        Name = "John Doe",
        PhoneNumber = "1112223333",
        Address = "test 123 Ave",
        State = "IL",
        PostalCode = "23422",
        City = "Chicago"
      }, "Admin@123").GetAwaiter().GetResult();

      ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@gmail.com");
      _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
    }

    return;
  }
}
