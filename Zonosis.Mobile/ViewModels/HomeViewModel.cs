using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zonosis.Mobile.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {

        private readonly IRepository _repository;
        private readonly CommonService _commonService;
        private readonly AuthService _authService;
        private const string BaseUrl = "https://zonosisapi.azurewebsites.net/";

        public HomeViewModel(IRepository repository, CommonService commonService, AuthService authService)
        {
            _repository = repository;
            _commonService = commonService;
            _authService = authService;
            _commonService.LoginStatusChanged += OnLoginStatusChanged;
            SetUserInfo();
        }

        private void OnLoginStatusChanged(object? sender, EventArgs e)
        {
            SetUserInfo();
        }
        private void SetUserInfo()
        {
            if (_authService.IsLoggedIn)
            {
                var userInfo = _authService.GetUser();
                UserName = userInfo.Name;
                _commonService.SetToken(userInfo.Token);
            }
            else
            {
                UserName = "Desconocido";
            }
        }

        [ObservableProperty]
        private IEnumerable<PetListDto> _newlyAdded = Enumerable.Empty<PetListDto>();

        [ObservableProperty]
        private IEnumerable<PetListDto> _poplar = Enumerable.Empty<PetListDto>();

        [ObservableProperty]
        private IEnumerable<PetListDto> _random = Enumerable.Empty<PetListDto>();

        [ObservableProperty]
        private string _userName = "Sin sesion";

        private bool _isInitialized;

        public async Task InitializeAsync()
        {
            if (_isInitialized)
                return;

            IsBusy = true;
            try
            {
                await Task.Delay(100);
                // Llamar a NewlyAddedTask y asignar el resultado a NewlyAdded
                NewlyAdded = await NewlyAddedTask(2);
                Poplar = await PopularPetsTask(2);
                Random = await RandomPetsTask(2);

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<List<PetListDto>> NewlyAddedTask(int count)
        {
            var responseHttp = await _repository.Get<List<Pet>>(BaseUrl, $"/api/mascotas/new/{count}");

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await ShowAlertAsync("Error", message!);
                return new List<PetListDto>(); // Retorna una lista vacía en caso de error
            }

            // Proyección de Pet a PetListDto
            var petsListDto = responseHttp.Response?
                .Select(pet => new PetListDto
                {
                    Id = pet.Id,
                    Name = pet.Name,
                    Image = pet.Image,
                    Price = pet.Price,
                    Raza = pet.Raza
                })
                .ToList() ?? new List<PetListDto>();

            return petsListDto;
        }

        private async Task<List<PetListDto>> PopularPetsTask(int count)
        {
            var responseHttp = await _repository.Get<List<Pet>>(BaseUrl, $"/api/mascotas/popular/{count}");

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await ShowAlertAsync("Error", message!);
                return new List<PetListDto>(); // Retorna una lista vacía en caso de error
            }

            // Proyección de Pet a PetListDto
            var petsListDto = responseHttp.Response?
                .Select(pet => new PetListDto
                {
                    Id = pet.Id,
                    Name = pet.Name,
                    Image = pet.Image,
                    Price = pet.Price,
                    Raza = pet.Raza
                })
                .ToList() ?? new List<PetListDto>();

            return petsListDto;
        }

        private async Task<List<PetListDto>> RandomPetsTask(int count)
        {
            var responseHttp = await _repository.Get<List<Pet>>(BaseUrl, $"/api/mascotas/random/{count}");

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await ShowAlertAsync("Error", message!);
                return new List<PetListDto>(); // Retorna una lista vacía en caso de error
            }

            // Proyección de Pet a PetListDto
            var petsListDto = responseHttp.Response?
                .Select(pet => new PetListDto
                {
                    Id = pet.Id,
                    Name = pet.Name,
                    Image = pet.Image,
                    Price = pet.Price,
                    Raza = pet.Raza
                })
                .ToList() ?? new List<PetListDto>();

            return petsListDto;
        }

        //[RelayCommand]
        //private async Task GoToDetailsPage(int petId) =>
        //    await GoToAsync($"{nameof(DetailsPage)}?{nameof(DetailsViewModel.PetId)}={petId}");
    }
}
