using System.ComponentModel.DataAnnotations;
namespace Movies.Domain.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string Title { get; set; }

        public int Year { get; set; }

        public double Rate { get; set; }

        [Required, MaxLength(2500)]
        public string Storeline { get; set; }
        public byte[]? Poster { get; set; }

        public ICollection<MovieActor> MovieActors { get; set; }

        public string Director { get; set; }

        public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
     
    }
}
