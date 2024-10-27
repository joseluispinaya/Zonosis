namespace Zonosis.Mobile.Models
{
    public class Pet
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
    }
}
