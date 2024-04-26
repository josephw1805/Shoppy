using Shoppy.Models;

namespace Shoppy.DataAccess;

public class ApplicationUserRepository(ApplicationDbContext db) : Repository<ApplicationUser>(db), IApplicationUserRepository
{
  private readonly ApplicationDbContext _db = db;
}