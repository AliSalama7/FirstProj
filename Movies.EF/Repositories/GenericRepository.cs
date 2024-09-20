using Microsoft.EntityFrameworkCore;
using Movies.Domain.Interfaces;
using Movies.EF.Data;
using System.Linq.Expressions;
namespace Movies.EF.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly MoviesDbContext _context;

        public GenericRepository(MoviesDbContext context)
        {
            _context = context;
        }

        public void Add(T entity)
        {
            _context.Add(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int? id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task<IEnumerable<T>> GetAllAsync<TKey>(Expression<Func<T, TKey>> orderBy)
        {
            return await _context.Set<T>().OrderBy(orderBy).ToListAsync();
        }
        public async Task<IEnumerable<T>> GetAllAsyncDec<TKey>(Expression<Func<T, TKey>> orderBy)
        {
            return await _context.Set<T>().OrderByDescending(orderBy).ToListAsync();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task<T> Find(Expression<Func<T, bool>> match, string[] Includes)
        {
            IQueryable<T> query = _context.Set<T>();
            if (Includes != null)
                foreach (var Include in Includes)
                    query = query.Include(Include);
            return await query.SingleOrDefaultAsync(match);
        }

        public void Delete(T entity)
        {
            _context.Remove(entity);
        }
    }
}

