namespace Shoppy.DataAccess;

public interface IUnitOfWork
{
  ICategoryRepository Category { get; }
  IProductRepository Product { get; }

  void Save();
}
