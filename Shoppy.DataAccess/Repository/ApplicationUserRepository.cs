using Shoppy.Models;

namespace Shoppy.DataAccess;

public class ApplicationUserRepository(ApplicationDbContext db) : Repository<ApplicationUser>(db), IApplicationUserRepository
{
  private readonly ApplicationDbContext _db = db;

  public void Update(ApplicationUser applicationUser)
  {
    _db.ApplicationUsers.Update(applicationUser);
  }
}