namespace Shoppy.DataAccess;

public interface IUnitOfWork
{
  ICategoryRepository Category { get; }

  void Save();
}
