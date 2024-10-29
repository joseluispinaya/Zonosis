namespace Zonosis.Mobile.Models
{
    public partial class PetSlim : ObservableObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }

        [ObservableProperty]
        private bool _isFavorite;
    }
}
