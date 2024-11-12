using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Zonosis.Shared.Entities;
using Zonosis.Web.Repositories;

namespace Zonosis.Web.Pages.Auth
{
    public partial class UserIndex
    {
        public List<User>? Users { get; set; }
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            var responseHppt = await Repository.Get<List<User>>("/api/accounts/usuarios");
            if (responseHppt.Error)
            {
                var message = await responseHppt.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            Users = responseHppt.Response;
        }
    }
}