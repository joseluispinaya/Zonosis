namespace Zonosis.Mobile.ViewModels
{
    public partial class AllPetsViewModel : BaseViewModel
    {
        private readonly IRepository _repository;
        private const string BaseUrl = "https://zonosisapi.azurewebsites.net/";
        public AllPetsViewModel(IRepository repository)
        {
            _repository = repository;
        }
        [ObservableProperty]
        private IEnumerable<PetListDto> _pets = Enumerable.Empty<PetListDto>();

        [ObservableProperty]
        private bool _isRefreshing;

        private bool _isInitialized;

        public async Task InitializeAsync()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;

            await LoadAllPets(true);
        }

        private async Task LoadAllPets(bool initialLoad)
        {
            if (initialLoad)
                IsBusy = true;
            else
                IsRefreshing = true;
            try
            {
                await Task.Delay(100);
                var responseHttp = await _repository.Get<List<Pet>>(BaseUrl, "/api/mascotas/full");
                if (responseHttp.Error)
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await ShowAlertAsync("Error", message!);
                    return;
                }
                // Proyección de Pet a PetListDto
                Pets = responseHttp.Response?
                    .Select(pet => new PetListDto
                    {
                        Id = pet.Id,
                        Name = pet.Name,
                        Image = pet.Image,
                        Price = pet.Price,
                        Raza = pet.Raza
                    })
                    .ToList() ?? new List<PetListDto>();

                //Pets = petsListDto;
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Error al cargar mascotas", ex.Message);
            }
            finally
            {
                IsBusy = IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task LoadPets() => await LoadAllPets(false);
    }
}
