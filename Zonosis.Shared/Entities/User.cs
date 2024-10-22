using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Zonosis.Shared.Enumerations;

namespace Zonosis.Shared.Entities
{
    public class User : IdentityUser
    {

        [Display(Name = "Nombres")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Tipo de usuario")]
        public UserType UserType { get; set; }

        //public ICollection<UserFavorites>? UserFavoritos { get; set; }
        //public ICollection<UserAdoption>? UserAdopciones { get; set; }
    }
}
