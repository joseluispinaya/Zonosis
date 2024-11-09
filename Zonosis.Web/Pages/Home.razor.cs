using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Zonosis.Shared.Entities;
using Zonosis.Web.Repositories;

namespace Zonosis.Web.Pages
{
    public partial class Home
    {
        private int counter = 0;
        private bool isAuthenticated;
        public List<Pet>? Pets { get; set; }

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            //loading = true;
            int count = 8;
            var responseHppt = await Repository.Get<List<Pet>>($"/api/mascotas/new/{count}");
            if (responseHppt.Error)
            {
                var message = await responseHppt.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            Pets = responseHppt.Response;
            //loading = false;
        }

        protected async override Task OnParametersSetAsync()
        {
            await CheckIsAuthenticatedAsync();
            await LoadCounterAsync();
        }

        private async Task CheckIsAuthenticatedAsync()
        {
            var authenticationState = await authenticationStateTask;
            isAuthenticated = authenticationState.User.Identity!.IsAuthenticated;
        }
        private async Task LoadCounterAsync()
        {
            if (!isAuthenticated)
            {
                return;
            }

            var responseHttp = await Repository.Get<List<Pet>>("/api/users/favorites");
            if (responseHttp.Error)
            {
                return;
            }
            counter = responseHttp.Response!.Count;
            //counter = responseHttp.Response;
        }

        private async Task AddToCartAsync(int petId)
        {
            if (!isAuthenticated)
            {
                NavigationManager.NavigateTo("/Login");
                var toast1 = SweetAlertService.Mixin(new SweetAlertOptions
                {
                    Toast = true,
                    Position = SweetAlertPosition.BottomEnd,
                    ShowConfirmButton = false,
                    Timer = 3000
                });
                await toast1.FireAsync(icon: SweetAlertIcon.Error, message: "Debes haber iniciado sesión para poder agregar a favorito.");
                return;
            }

            var httpActionResponse = await Repository.PostFavoAsync<object>($"/api/users/favorites/{petId}");
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            await LoadCounterAsync();

            var toast2 = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });
            await toast2.FireAsync(icon: SweetAlertIcon.Success, message: "Mascota agregado a favoritos.");
        }
    }
}
