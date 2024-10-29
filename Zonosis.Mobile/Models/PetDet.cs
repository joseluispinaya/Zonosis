using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zonosis.Mobile.Models
{
    public partial class PetDet : ObservableObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public string? Raza { get; set; }

        [ObservableProperty]
        private bool _isFavorite;

        public string? Description { get; set; }

        [ObservableProperty]
        private AdoptionStatus _adoptionStatus;

        public string? GeneroDisplay { get; set; }
        public string? GeneroImage { get; set; }
        public string? Edad { get; set; }

        public string PrecioCade => $"Bs/ {Price:F2}";

        //public string PrecioCad => $"Bs/ {Price:F2}";
    }
}
