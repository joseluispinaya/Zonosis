namespace Zonosis.Mobile.ViewModels
{
    [QueryProperty(nameof(PetId), nameof(PetId))]
    public partial class DetailsViewModel : BaseViewModel
    {
        private readonly IRepository _repository;
        private readonly AuthService _authService;
        private const string BaseUrl = "https://zonosisapi.azurewebsites.net/";

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


        //private async Task<PetDetailDTO> FetchPetDetailsAsync(int petId)
        //{
        //    var response = await GetPetDetailsAsync(petId);

        //    return response ?? new PetDetailDTO();
        //}

        //private async Task<PetDetailDTO?> GetPetDetailsAsync(int petId)
        //{
        //    var response = await _repository.Get<PetDetailDTO>(BaseUrl, $"/api/mascotas/{petId}");

        //    return await HandleApiResponseAsync(response);
        //}

        //private async Task<PetDetailDTO?> HandleApiResponseAsync(HttpResponseWrapper<PetDetailDTO> response)
        //{
        //    if (response.Error)
        //    {
        //        var message = await response.GetErrorMessageAsync();
        //        await ShowAlertAsync("Error", message!);
        //        return null;
        //    }

        //    return response.Response;
        //}

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
    }
}
