using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Shoppy.DataAccess;

public class Repository<T> : IRepository<T> where T : class
{
  private readonly ApplicationDbContext _db;
  internal DbSet<T> dbSet;
  public Repository(ApplicationDbContext db)
  {
    _db = db;
    dbSet = _db.Set<T>();
  }

  public void Add(T entity)
  {
    dbSet.Add(entity);
  }

  public T Get(Expression<Func<T, bool>> filter)
  {
    IQueryable<T> query = dbSet;
    query = query.Where(filter);
    return query.FirstOrDefault();
  }

  public IEnumerable<T> GetAll()
  {
    IQueryable<T> query = dbSet;
    return [.. query];
  }

  public void Remove(T entity)
  {
    dbSet.Remove(entity);
  }

  public void RemoveRange(IEnumerable<T> entity)
  {
    dbSet.RemoveRange(entity);
  }
}
