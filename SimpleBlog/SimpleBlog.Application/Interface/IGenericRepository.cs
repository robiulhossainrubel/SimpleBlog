using System.Linq.Expressions;

namespace SimpleBlog.Application.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task Create(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        T GetById(int id);
        T Get(Expression<Func<T, bool>> expression, string? includeProperties = null, bool tracked = true);
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? expression = null, string? includeProperties = null);
    }
}
