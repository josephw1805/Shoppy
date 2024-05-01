using Shoppy.Models;

namespace Shoppy.DataAccess;

public interface IApplicationUserRepository : IRepository<ApplicationUser>
{
  public void Update(ApplicationUser applicationUser);
}
