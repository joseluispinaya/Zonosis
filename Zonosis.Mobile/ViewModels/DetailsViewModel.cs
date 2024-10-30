using Microsoft.AspNetCore.SignalR.Client;
using Zonosis.Shared;

namespace Zonosis.Mobile.ViewModels
{
    [QueryProperty(nameof(PetId), nameof(PetId))]
    public partial class DetailsViewModel : BaseViewModel, IAsyncDisposable
    {
        private readonly IRepository _repository;
        private readonly AuthService _authService;
        private const string BaseUrl = "https://zonosisapi.azurewebsites.net/";
        private HubConnection? _hubConnection;

        public DetailsViewModel(IRepository repository, AuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        [ObservableProperty]
        private int _petId;

        [ObservableProperty]
        private PetDet _petDetail = new();

        async partial void OnPetIdChanging(int value)
        {
            IsBusy = true;

            try
            {
                await Task.Delay(100);
                await ConfigureSignalRHubConnectionAsync(value);

                PetDetail = await FetchPetDetailsAsync(value);
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Error al obtener los detalles de la mascota", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<PetDet> FetchPetDetailsAsync(int petId)
        {
            //return await GetPetDetailsAsync(petId);
            return _authService.IsLoggedIn 
                ? await GetPetUserDetailsAsync(petId)
                : await GetPetDetailsAsync(petId);
        }

        private async Task<PetDet> GetPetUserDetailsAsync(int petId)
        {
            var userInfo = _authService.GetUser();
            var response = await _repository.GetUserByEmail<PetDetailDTO>(BaseUrl, $"/api/users/view-pet-details/{petId}", "bearer", userInfo.Token);

            return await HandleApiResponseAsync(response);
        }

        private async Task<PetDet> GetPetDetailsAsync(int petId)
        {
            var response = await _repository.Get<PetDetailDTO>(BaseUrl, $"/api/mascotas/{petId}");

            return await HandleApiResponseAsync(response);
        }

        private async Task<PetDet> HandleApiResponseAsync(HttpResponseWrapper<PetDetailDTO> response)
        {
            if (response.Error)
            {
                var message = await response.GetErrorMessageAsync();
                await ShowAlertAsync("Error", message!);
                return new PetDet();  // Retorna una instancia vacía en caso de error
            }
            return ConverterPet(response.Response!);
        }

        private PetDet ConverterPet(PetDetailDTO petDto)
        {
            return new PetDet
            {
                AdoptionStatus = petDto.AdoptionStatus,
                Edad = petDto.Edad,
                Raza = petDto.Raza,
                Description = petDto.Description,
                GeneroDisplay = petDto.GeneroDisplay,
                GeneroImage = petDto.GeneroImage,
                Id = petDto.Id,
                Image = petDto.Image,
                IsFavorite = petDto.IsFavorite,
                Name = petDto.Name,
                Price = petDto.Price
            };
        }

        private async Task ConfigureSignalRHubConnectionAsync(int currentPetId)
        {
            try
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(AppConstants.HubFullUrl)
                    .Build();

                _hubConnection.On<int>(nameof(IPetHubClient.PetIsBeingViewed), async petId =>
                {
                    if (currentPetId == petId)
                    {
                        await App.Current!.Dispatcher.DispatchAsync(() => ShowToastAsync("Alguien más está viendo esta mascota."));
                    }
                });

                _hubConnection.On<int>(nameof(IPetHubClient.PetAdopted), async petId =>
                {
                    if (currentPetId == petId)
                    {
                        PetDetail.AdoptionStatus = AdoptionStatus.Adoptado;
                        await App.Current!.Dispatcher.DispatchAsync(() => ShowToastAsync("Alguien adoptó esta mascota. No podrás adoptarla ahora"));
                    }
                });

                await _hubConnection.StartAsync();
                await _hubConnection.SendAsync(nameof(IPetHubServer.ViewingThisPet), currentPetId);

            }
            catch
            {
                // Eat out this exception
                // This is not an essential feature for this app
                // If there is some issue with this signalr connection, we can skip it
                // as the app will work fine without signalr as well
                //throw;
            }
        }

        [RelayCommand]
        private async Task GoBack() => await GoToAsync("..");

        [RelayCommand]
        private async Task ToggleFavorite()
        {
            if (!_authService.IsLoggedIn)
            {
                await ShowToastAsync("Debes iniciar sesión para marcar esta mascota como favorita");
                return;
            }
            PetDetail.IsFavorite = !PetDetail.IsFavorite;

            try
            {
                IsBusy = true;
                var userInfo = _authService.GetUser();
                var responseHttp = await _repository.PostFavoAsync<object>(BaseUrl, $"/api/users/favorites/{PetId}", "bearer", userInfo.Token);
                if (responseHttp.Error)
                {
                    IsBusy = false;
                    var message = await responseHttp.GetErrorMessageAsync();
                    await ShowAlertAsync("Error", message!);
                    return;
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                //Revert 
                PetDetail.IsFavorite = !PetDetail.IsFavorite;

                await ShowAlertAsync("Error al alternar el estado de favorito", ex.Message);
            }
        }
        [RelayCommand]
        private async Task AdoptNowAsync()
        {
            if (!_authService.IsLoggedIn)
            {
                if (await ShowConfirmAsync("Sin sesion", "Inicia session para adoptar" + Environment.NewLine + "Deseas iniciar sesion?", "Yes", "No"))
                {
                    await GoToAsync($"//{nameof(LoginRegisterPage)}");
                }
                return;
            }
            IsBusy = true;

            try
            {
                var userInfo = _authService.GetUser();
                var responseHttp = await _repository.PostFavoAsync<object>(BaseUrl, $"/api/users/adopt/{PetId}", "bearer", userInfo.Token);
                if (!responseHttp.Error)
                {
                    PetDetail.AdoptionStatus = AdoptionStatus.Adoptado;
                    if (_hubConnection is not null)
                    {
                        try
                        {
                            await _hubConnection.SendAsync(nameof(IPetHubServer.PetAdopted), PetId);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    await GoToAsync(nameof(AdoptionSuccessPage));
                }
                else
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await ShowAlertAsync("Error en la adopción", message!);
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Error en la adopción", ex.Message);
                IsBusy = false;
            }
        }

        //public void Dispose()
        //{
        //    throw new NotImplementedException();
        //}

        public async ValueTask DisposeAsync() => await StopHubConnection();

        public async Task StopHubConnection()
        {
            if (_hubConnection is not null)
            {
                try
                {
                    await _hubConnection.SendAsync(nameof(IPetHubServer.ReleaseViewingThisPet), PetId);
                    await _hubConnection.StopAsync();
                }
                catch (Exception)
                {
                    // Skip this exception
                }
            }
        }
    }
}
