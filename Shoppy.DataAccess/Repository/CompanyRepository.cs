using Shoppy.Models;

namespace Shoppy.DataAccess;

public class CompanyRepository(ApplicationDbContext db) : Repository<Company>(db), ICompanyRepository
{
  private readonly ApplicationDbContext _db = db;
  public void Update(Company obj)
  {
    _db.Companies.Update(obj);
  }
}
