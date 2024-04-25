using Shoppy.Models;

namespace Shoppy.DataAccess;

public interface ICompanyRepository : IRepository<Company>
{
  void Update(Company obj);
}
