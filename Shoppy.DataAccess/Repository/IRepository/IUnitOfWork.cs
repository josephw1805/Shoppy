namespace Shoppy.DataAccess;

public interface IUnitOfWork
{
  ICategoryRepository Category { get; }
  IProductRepository Product { get; }
  ICompanyRepository Company { get; }
  IApplicationUserRepository ApplicationUser { get; }
  IShoppingCartRepository ShoppingCart { get; }
  IOrderHeaderRepository OrderHeader { get; }
  IOrderDetailRepository OrderDetail { get; }

  void Save();
}
