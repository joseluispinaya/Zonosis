using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zonosis.Mobile.ViewModels
{
    public partial class MyAdoptionsViewModel : BaseViewModel
    {
        private readonly IRepository _repository;
        private readonly AuthService _authService;
        private const string BaseUrl = "https://zonosisapi.azurewebsites.net/";

        public MyAdoptionsViewModel(IRepository repository, AuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        [ObservableProperty]
        private IEnumerable<PetListDto> _myAdoptions = Enumerable.Empty<PetListDto>();

        public async Task InitializeAsync()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await ShowAlertAsync("Error de Red", "Verifique la conexion a Internet");
                return;
            }

            if (!_authService.IsLoggedIn)
            {
                await ShowToastAsync("Necesitas logearte para ver tus adopciones");
                return;
            }
            try
            {
                IsBusy = true;
                await Task.Delay(100);
                var userInfo = _authService.GetUser();
                var responseHttp = await _repository.GetUserByEmail<List<Pet>>(BaseUrl, "/api/users/adoptions", "bearer", userInfo.Token);
                if (!responseHttp.Error)
                {
                    MyAdoptions = responseHttp.Response?
                        .Select(pet => new PetListDto
                        {
                            Id = pet.Id,
                            Name = pet.Name,
                            Image = pet.Image,
                            Price = pet.Price,
                            Raza = pet.Raza
                        }) ?? Enumerable.Empty<PetListDto>();
                }
                else
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await ShowAlertAsync("Error", message!);
                }
            }
            //catch (HttpRequestException)
            //{
            //    await ShowAlertAsync("Error de red", "No se pudo conectar al servidor. Inténtalo nuevamente.");
            //}
            catch (Exception ex)
            {
                await ShowAlertAsync("Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
