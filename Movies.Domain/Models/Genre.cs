using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Movies.Domain.Models
{
    public class Genre
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        public ICollection<MovieGenre> MovieGenres { get; set; }

    }
}
