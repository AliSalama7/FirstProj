using Movies.Domain.Models;
using System.ComponentModel.DataAnnotations;
namespace MoviesApp.Models
{
    public class MovieFormViewModel
    {
        public int  Id{ get; set; }
        [Required, StringLength(250)]
        public string Title { get; set; }

        public int Year { get; set; }

        public string Director { get; set; }
        [Range (1,10)]
        public double Rate { get; set; }

        [Required, StringLength(2500)]
        public string Storeline { get; set; }

        [Display(Name ="Select Poster........")]
        public byte[]? Poster { get; set; }
        public List<int>? GenresId { get; set; }
        public IEnumerable<Genre> Genres { get; set; } = new List<Genre> ();

    }
}
