using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zonosis.Mobile.DTOs
{
    public class PetDetailDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public string? Raza { get; set; }
        public bool IsFavorite { get; set; }
        public string? Description { get; set; }
        public int Genero { get; set; }
        public DateTime DateNacido { get; set; }
        public AdoptionStatus AdoptionStatus { get; set; }
        public string? GeneroDisplay { get; set; }
        public string? GeneroImage { get; set; }
        public string? Edad { get; set; }

        public string PrecioCade => $"Bs/ {Price:F2}";
    }
}
