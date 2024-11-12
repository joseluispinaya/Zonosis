using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Zonosis.Shared.Entities;
using Zonosis.Web.Repositories;

namespace Zonosis.Web.Pages.Pets
{
    public partial class FavoritosPet
    {
        public List<Pet>? Pets { get; set; }

        //[Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadDetalleAsync();
        }

        private async Task LoadDetalleAsync()
        {
            var responseHppt = await Repository.Get<List<Pet>>("/api/users/favorites");
            if (responseHppt.Error)
            {
                var message = await responseHppt.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            Pets = responseHppt.Response;
        }

        private async Task RemovToPetAsync(int petId)
        {

            var httpActionResponse = await Repository.PostFavoAsync<object>($"/api/users/favorites/{petId}");
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            await LoadDetalleAsync();

            var toast2 = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });
            await toast2.FireAsync(icon: SweetAlertIcon.Success, message: "Mascota removida de favoritos.");
        }
    }
}