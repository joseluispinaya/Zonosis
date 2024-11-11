using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Zonosis.Shared.DTOs;
using Zonosis.Shared.Entities;
using Zonosis.Web.Repositories;

namespace Zonosis.Web.Pages.Pets
{
    public partial class MascotaDetalle
    {
        private bool loading = true;
        private PetDetailDTO? pet;
        private bool isAuthenticated;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter] public int PetId { get; set; }
        [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

        //protected async override Task OnParametersSetAsync()
        //{
        //    await CheckIsAuthenticatedAsync();
        //}


        protected override async Task OnInitializedAsync()
        {
            //await LoadPetAsync();
            await CheckIsAuthenticatedAsync();
            await LoadDetalleAsync();
        }

        private async Task CheckIsAuthenticatedAsync()
        {
            var authenticationState = await AuthenticationStateTask;
            isAuthenticated = authenticationState.User.Identity!.IsAuthenticated;
        }

        private async Task LoadDetalleAsync()
        {
            loading = true;

            var endpoint = isAuthenticated
                ? $"/api/users/view-pet-details/{PetId}"
                : $"/api/mascotas/{PetId}";

            var httpActionResponse = await Repository.Get<PetDetailDTO>(endpoint);

            if (httpActionResponse.Error)
            {
                loading = false;
                var message = await httpActionResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            pet = httpActionResponse.Response!;
            loading = false;
        }

        

        private async Task LoadPetAsync()
        {
            loading = true;
            var httpActionResponse = await Repository.Get<PetDetailDTO>($"/api/mascotas/{PetId}");

            if (httpActionResponse.Error)
            {
                loading = false;
                var message = await httpActionResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            pet = httpActionResponse.Response!;
            loading = false;
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
            NavigationManager.NavigateTo("/destacados");

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