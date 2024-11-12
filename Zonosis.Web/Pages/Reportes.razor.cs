using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Zonosis.Shared.DTOs;
using Zonosis.Web.Repositories;
using Zonosis.Web.Services;

namespace Zonosis.Web.Pages
{
    public partial class Reportes
    {
        private string Texto = "";
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private ILoginService LoginService { get; set; } = null!;

        private async Task Export()
        {
            if (string.IsNullOrEmpty(Texto))
            {
                Texto = "1";
            }
            var httpActionResponse = await Repository.Get<PetDetailDTO>($"/api/mascotas/{Texto}");
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            await LoginService.ReportPetDtAsync(httpActionResponse.Response!);

            //if (string.IsNullOrEmpty(Texto))
            //{
            //    Texto = "sin parametro";
            //}
            //await LoginService.ReportAsync(Texto);
        }
    }
}