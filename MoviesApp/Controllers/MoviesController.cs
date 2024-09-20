using Microsoft.AspNetCore.Mvc;
using Movies.Domain.Interfaces;
using Movies.Domain.Models;
using MoviesApp.Models;
using NToastNotify;
namespace MoviesApp.Controllers
{
    public class MoviesController : Controller
    {
        private List<string> _allowedExtenstions = new List<string> { ".jpg", ".png",".jpeg" };
        private long _maxAllowedPosterSize = 1048576;
        private readonly IToastNotification _toastnotification;
        public IGenericRepository<Movie> _moviesRepository { get; set; }
        public IMoviesRepository moviesRepository { get; set; }
        public IGenericRepository<Genre> _genresRepository { get; set; }
        public MoviesController(IGenericRepository<Movie> MoviesRepository,IMoviesRepository moviesRepository, IGenericRepository<Genre> GenresRepository, IToastNotification toastnotification)
        {
            _moviesRepository = MoviesRepository;
            _genresRepository = GenresRepository;
            _toastnotification = toastnotification;
            this.moviesRepository = moviesRepository;
        }
        public async Task<IActionResult> Index()
        {
            var movies = await _moviesRepository.GetAllAsyncDec(m => m.Rate);
            return View(movies);
        }
        public async Task<IActionResult> Create()
        {
            var ViewModel = new MovieFormViewModel
            {
                Genres = await _genresRepository.GetAllAsync(m => m.Name)
            };
            return View("MovieForm", ViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _genresRepository.GetAllAsync(m => m.Name);
                return View("MovieForm", model);
            }

            var files = Request.Form.Files;

            if (!files.Any())
            {
                model.Genres = await _genresRepository.GetAllAsync(m => m.Name);
                ModelState.AddModelError("Poster", "Please select movie poster!");
                return View("MovieForm", model);
            }
            var poster = files.FirstOrDefault();
            if (!_allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Genres = await _genresRepository.GetAllAsync(m => m.Name);
                ModelState.AddModelError("Poster", "Only .PNG, .JPG images are allowed!");
                return View("MovieForm", model);
            }

            if (poster.Length > _maxAllowedPosterSize)
            {
                model.Genres = await _genresRepository.GetAllAsync(m => m.Name);
                ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB!");
                return View("MovieForm", model);
            }

            using var dataStream = new MemoryStream();

            await poster.CopyToAsync(dataStream);

            var movies = new Movie
            {
                Title = model.Title,
                Year = model.Year,
                Rate = model.Rate,
                Storeline = model.Storeline,
                Poster = dataStream.ToArray(),
                Director = model.Director,
                MovieGenres = model.GenresId.Select(genreid => new MovieGenre { GenreId = genreid}).ToList()
            };
            _moviesRepository.Add(movies);
            _moviesRepository.Save();
            _toastnotification.AddSuccessToastMessage("Movie Created Successfully");
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _moviesRepository.GetByIdAsync(id);

            if (movie == null)
                return NotFound();

            var ViewModel = new MovieFormViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Year = movie.Year,
                Rate = movie.Rate,
                Storeline = movie.Storeline,
                Poster = movie.Poster,
                Director = movie.Director,
                Genres = await _genresRepository.GetAllAsync(),
                GenresId = movie.MovieGenres?.Select(mg => mg.GenreId).ToList()
            };

            return View("MovieForm", ViewModel);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(MovieFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _genresRepository.GetAllAsync(m => m.Name);
                return View("MovieForm", model);
            }
            var movie = await _moviesRepository.GetByIdAsync(model.Id);

            if (movie == null)
                return NotFound();

            var files = Request.Form.Files;

            if (files.Any())
            {
                var poster = files.FirstOrDefault();
                using var dataStream = new MemoryStream();
                await poster.CopyToAsync(dataStream);
                model.Poster = dataStream.ToArray();
                if (!_allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Genres = await _genresRepository.GetAllAsync(m => m.Name);
                    ModelState.AddModelError("Poster", "Only .PNG, .JPG images are allowed!");
                    return View("MovieForm", model);
                }

                if (poster.Length > _maxAllowedPosterSize)
                {
                    model.Genres = await _genresRepository.GetAllAsync(m => m.Name);
                    ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB!");
                    return View("MovieForm", model);
                }
                movie.Poster = model.Poster;
            }
                movie.Title = model.Title;
                movie.Year = model.Year;
                movie.Rate = model.Rate;
                movie.Storeline = model.Storeline;
                movie.Director = model.Director;
                if (model.GenresId != null) { 
                foreach (var genreId in model.GenresId)
                {
                    var existingMovieGenre = await moviesRepository.GetMovieGenreAsync(movie.Id, genreId);
                    if (existingMovieGenre != null)
                    {
                        movie.MovieGenres.Add(existingMovieGenre);
                    }
                    else
                    {
                        movie.MovieGenres.Add(new MovieGenre { MovieId = movie.Id, GenreId = genreId });
                    }
                } 
        }

            _moviesRepository.Save();
            _toastnotification.AddSuccessToastMessage("Movie Updated Successfully");
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _moviesRepository.Find(m => m.Id == id, new[] { "MovieGenres.Genre" , "MovieActors.Actor"} );

            if (movie == null)
                return NotFound();

            return View(movie);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _moviesRepository.GetByIdAsync(id);

            if (movie == null)
                return NotFound();

            _moviesRepository.Delete(movie);
            _moviesRepository.Save();
            return Ok();
        }
    }
}