using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Application.Interface;
using SimpleBlog.Infrastructure.Data;

namespace SimpleBlog.Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly BlogDbContext _context;
        private readonly DbSet<T> _table;

        public GenericRepository(BlogDbContext context)
        {
            _context = context;
            _table = _context.Set<T>();
        }
        public async Task Create(T entity)
        {
            try
            {
                await _table.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Delete(T entity)
        {
            try
            {
                _table.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T Get(Expression<Func<T, bool>> expression, string? includeProperties = null, bool tracked = true)
        {
            try
            {
                IQueryable<T> query = _table;

                if (tracked == false)
                {
                    query = query.AsNoTracking();
                }

                if (includeProperties != null)
                {
                    foreach (var property in includeProperties.Split(","))
                    {
                        query = query.Include(property);
                    }
                }

                var entity = query.FirstOrDefault(expression);

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? expression = null, string? includeProperties = null)
        {
            try
            {
                IQueryable<T> query = _table;

                if (expression != null)
                {
                    query = query.Where(expression);
                }

                if (includeProperties != null)
                {
                    foreach (var property in includeProperties.Split(","))
                    {
                        query = query.Include(property);
                    }
                }

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T GetById(int id)
        {
            try
            {
                return _table.Find(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Update(T entity)
        {
            try
            {
                _table.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
