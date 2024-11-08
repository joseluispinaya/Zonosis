using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zonosis.Shared.Enumerations;

namespace Zonosis.Shared.DTOs
{
    public class PetDto
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(25, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; } = null!;

        [Display(Name = "Foto")]
        public string? Image { get; set; }

        [Display(Name = "Raza")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Raza { get; set; } = null!;
        //Breed

        [Display(Name = "Tipo de Genero")]
        public Genero Genero { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Precio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public decimal Price { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha Nacido")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public DateTime DateNacido { get; set; }


        [Display(Name = "Descripción")]
        [MaxLength(250, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Description { get; set; } = null!;

        //public int Views { get; set; }

        //public AdoptionStatus AdoptionStatus { get; set; }

        //public bool IsActive { get; set; }
    }
}
