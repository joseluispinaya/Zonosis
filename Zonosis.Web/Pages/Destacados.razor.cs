using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Zonosis.Shared.Entities;
using Zonosis.Web.Repositories;

namespace Zonosis.Web.Pages
{
    public partial class Destacados
    {
        private string carousel = Guid.NewGuid().ToString("N");
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        public List<Pet>? Pets { get; set; }

        protected override async Task OnInitializedAsync()
        {
            int count = 9;
            var responseHppt = await Repository.Get<List<Pet>>($"/api/mascotas/new/{count}");
            if (responseHppt.Error)
            {
                var message = await responseHppt.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            Pets = responseHppt.Response;
        }
    }
}