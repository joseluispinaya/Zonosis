using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zonosis.Mobile.DTOs
{
    public class PetListDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public string? Raza { get; set; }
        //Breed

        public string PrecioCad => $"Bs/ {Price:F2}";
    }
}
