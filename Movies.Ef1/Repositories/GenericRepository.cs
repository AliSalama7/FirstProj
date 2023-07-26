using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Movies.Domain.Interfaces;
using Movies.Domain.Models;
using Movies.EF.Data;
using NToastNotify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
            _context.SaveChanges();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
      
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, object>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = _context.Set<T>();

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                query = query.OrderBy(orderBy);
            }

            return await query.ToListAsync();
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
        //Movie
        public async Task<IEnumerable<T>> GetAllAsyncDec1()
        {
            return (IEnumerable<T>)await _context.Movies.Include(m => m.MovieGenres).ThenInclude(m => m.Genre).ToListAsync();
        }

        public async Task<MovieGenre> GetMovieGenreAsync(int movieId, int genreId)
        {
            return await _context.MovieGenres.FirstOrDefaultAsync(mg => mg.MovieId == movieId && mg.GenreId == genreId);
        }

        public void Update()
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
            _context.SaveChanges();
        }

    }
}

