using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zonosis.Shared.Enumerations;

namespace Zonosis.Shared.DTOs
{
    public class PetDetailRelDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public string? Raza { get; set; }
        public Genero Genero { get; set; }
        public decimal Price { get; set; }
        public DateTime DateNacido { get; set; }
        public string? Description { get; set; }
        public int Views { get; set; }
        public AdoptionStatus AdoptionStatus { get; set; }
        public bool IsActive { get; set; }
        public string? GeneroDisplay => Genero.ToString();
        public string? Edad
        {
            get
            {
                var diff = DateTime.Now.Subtract(DateNacido);
                var days = diff.Days;
                return days switch
                {
                    < 30 => days + " dias",
                    >= 30 and <= 31 => "1 mes",
                    < 365 => Math.Floor(diff.TotalDays / 30) + " meses",
                    365 => "1 Año",
                    _ => Math.Floor(diff.TotalDays / 365) + " Años"
                };
            }
        }
    }
}
