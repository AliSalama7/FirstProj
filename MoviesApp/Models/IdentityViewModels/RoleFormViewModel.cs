using System.ComponentModel.DataAnnotations;

namespace MoviesApp.Models.IdentityViewModels
{
    public class RoleFormViewModel
    {
        [Required, StringLength(256)]
        public string Name { get; set; }
    }
}
