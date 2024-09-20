using Microsoft.AspNetCore.Mvc;
using Movies.Domain.Interfaces;
using Movies.Domain.Models;
using MoviesApp.Models;
using System.Diagnostics;
namespace MoviesApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public IGenericRepository<Movie> _moviesRepository { get; set; }
        public IGenericRepository<Genre> _genresRepository { get; set; }
        public IMoviesRepository moviesRepository { get; set; }
        public HomeController(ILogger<HomeController> logger , IGenericRepository<Movie> movieRepository, IGenericRepository<Genre> genresRepository, IMoviesRepository moviesRepository)
        {
            _logger = logger;
            _moviesRepository = movieRepository;
            _genresRepository = genresRepository;
            this.moviesRepository = moviesRepository;
        }
        public async Task<IActionResult> Index(string term)
        {
            var movies = await moviesRepository.GetAllAsyncDec1();
            if (!string.IsNullOrWhiteSpace(term))
            {
                movies = movies.Where(m => m.Title.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            foreach(var movie in movies)
            {
                var genres = movie.MovieGenres;
            }
            return View(movies);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}