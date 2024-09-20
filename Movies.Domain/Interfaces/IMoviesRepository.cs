using Movies.Domain.Models;

namespace Movies.Domain.Interfaces
{
    public interface IMoviesRepository
    {
        Task<IEnumerable<Movie>> GetAllAsyncDec1();
        Task<MovieGenre> GetMovieGenreAsync(int movieId, int genreId);
    }
}
