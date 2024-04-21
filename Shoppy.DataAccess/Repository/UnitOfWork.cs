namespace Shoppy.DataAccess;

public class UnitOfWork : IUnitOfWork
{
  private readonly ApplicationDbContext _db;
  public ICategoryRepository Category { get; private set; }
  public UnitOfWork(ApplicationDbContext db)
  {
    _db = db;
    Category = new CategoryRepository(_db);
  }

  public void Save()
  {
    _db.SaveChanges();
  }
}
