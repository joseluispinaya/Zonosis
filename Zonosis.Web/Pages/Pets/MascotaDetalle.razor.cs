using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Zonosis.Shared.DTOs;
using Zonosis.Shared.Entities;
using Zonosis.Web.Repositories;

namespace Zonosis.Web.Pages.Pets
{
    public partial class MascotaDetalle
    {
        private bool loading = true;
        private PetDetailDTO? pet;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter] public int PetId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadPetAsync();
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