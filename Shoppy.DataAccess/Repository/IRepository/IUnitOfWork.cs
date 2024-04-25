namespace Shoppy.DataAccess;

public interface IUnitOfWork
{
  ICategoryRepository Category { get; }
  IProductRepository Product { get; }
  ICompanyRepository Company { get; }

  void Save();
}
