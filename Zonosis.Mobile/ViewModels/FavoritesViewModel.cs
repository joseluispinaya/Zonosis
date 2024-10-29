using System.Collections.ObjectModel;

namespace Zonosis.Mobile.ViewModels
{
    public partial class FavoritesViewModel : BaseViewModel
    {
        private readonly IRepository _repository;
        private readonly AuthService _authService;
        private const string BaseUrl = "https://zonosisapi.azurewebsites.net/";
        public FavoritesViewModel(IRepository repository, AuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        [ObservableProperty]
        public ObservableCollection<PetSlim> _pets = new();

        public async Task InitializeAsync()
        {
            if (!_authService.IsLoggedIn)
            {
                await ShowToastAsync("iniciar sesión para ver tus mascotas favoritas");
                return;
            }

            IsBusy = true;

            try
            {
                var userInfo = _authService.GetUser();
                var responseHttp = await _repository.GetUserByEmail<List<Pet>>(BaseUrl, "/api/users/favorites", "bearer", userInfo.Token);
                Pets = responseHttp.Error
                    ? new ObservableCollection<PetSlim>()
                    : ConvertToPetSlimCollection(responseHttp.Response);

                if (responseHttp.Error)
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await ShowAlertAsync("Error", message!);
                }

                //IsBusy = true;
                //var userInfo = _authService.GetUser();
                //var responseHttp = await _repository.GetUserByEmail<List<Pet>>(BaseUrl, "/api/users/favorites", "bearer", userInfo.Token);
                //if (responseHttp.Error)
                //{
                //    var message = await responseHttp.GetErrorMessageAsync();
                //    await ShowAlertAsync("Error", message!);
                //    return;
                //}

                //Pets = new ObservableCollection<PetSlim>(
                //    responseHttp.Response?
                //    .Select(p => new PetSlim
                //    {
                //        Id = p.Id,
                //        Image = p.Image,
                //        IsFavorite = true,
                //        Name = p.Name
                //    }) ?? Enumerable.Empty<PetSlim>()
                //);
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Error al buscar mascotas", ex.Message);
                Pets = new ObservableCollection<PetSlim>(); // Asignación en caso de excepción
            }
            finally
            {
                IsBusy = false;
            }
        }

        private ObservableCollection<PetSlim> ConvertToPetSlimCollection(List<Pet>? pets)
        {
            return new ObservableCollection<PetSlim>(
                pets?.Select(p => new PetSlim
                {
                    Id = p.Id,
                    Image = p.Image,
                    IsFavorite = true,
                    Name = p.Name
                }) ?? Enumerable.Empty<PetSlim>()
            );
        }

        [RelayCommand]
        private async Task ToggleFavoriteAsync(int petId)
        {
            try
            {
                var pet = Pets.FirstOrDefault(p => p.Id == petId);
                if (pet is not null)
                {
                    pet.IsFavorite = false;
                    IsBusy = true;
                    var userInfo = _authService.GetUser();
                    var responseHttp = await _repository.PostFavoAsync<object>(BaseUrl, $"/api/users/favorites/{petId}", "bearer", userInfo.Token);
                    if (responseHttp.Error)
                    {
                        //IsBusy = false;
                        var message = await responseHttp.GetErrorMessageAsync();
                        await ShowAlertAsync("Error", message!);
                        return;
                    }
                    Pets.Remove(pet);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
