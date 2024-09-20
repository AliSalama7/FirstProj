using Microsoft.EntityFrameworkCore;
using Movies.Domain.Interfaces;
using Movies.Domain.Models;
using Movies.EF.Data;

namespace Movies.EF.Repositories
{
    public class MoviesRepository : IMoviesRepository
    {
        private readonly MoviesDbContext _context;
        public MoviesRepository(MoviesDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Movie>> GetAllAsyncDec1()
        {
            return await _context.Movies.Include(m => m.MovieGenres).ThenInclude(m => m.Genre).ToListAsync();
        }

        public async Task<MovieGenre> GetMovieGenreAsync(int movieId, int genreId)
        {
            return await _context.MovieGenres.FirstOrDefaultAsync(mg => mg.MovieId == movieId && mg.GenreId == genreId);
        }

    }
}
