using Microsoft.AspNetCore.Identity;
using Movies.Domain.Models;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Movies.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int? id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync<TKey>(Expression<Func<T, TKey>> orderBy);
        Task<IEnumerable<T>> GetAllAsyncDec<TKey>(Expression<Func<T, TKey>> orderBy);
        void Delete(T entity);
        Task<T> Find(Expression<Func<T, bool>> match, string[] Includes);
        void Add(T entity);
        void Update();

        //IMoviesRepository
        Task<IEnumerable<T>> GetAllAsyncDec1();
        Task<MovieGenre> GetMovieGenreAsync(int movieId, int genreId);

      

    }
}
