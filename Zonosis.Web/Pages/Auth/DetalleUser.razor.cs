using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Zonosis.Shared.DTOs;
using Zonosis.Shared.Entities;
using Zonosis.Web.Repositories;
using Zonosis.Web.Services;

namespace Zonosis.Web.Pages.Auth
{
    public partial class DetalleUser
    {
        private List<Pet>? pets;
        private User? user;
        private bool loading = true;

        //[Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private ILoginService LoginService { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter] public string? UserId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            //await LoadAsync();
        }

        private async Task LoadDataAsync()
        {
            loading = true;

            // Cargar detalles del usuario
            var userResponse = await Repository.Get<List<User>>("/api/accounts/usuarios");
            if (userResponse.Error)
            {
                loading = false;
                var message = await userResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            user = userResponse.Response?.FirstOrDefault(u => u.Id == UserId);
            if (user == null)
            {
                await SweetAlertService.FireAsync("Error", "Usuario no encontrado", SweetAlertIcon.Error);
                loading = false;
                return;
            }

            // Cargar mascotas favoritas del usuario
            var petResponse = await Repository.Get<List<Pet>>($"/api/accounts/favoritos/{UserId}");
            if (petResponse.Error)
            {
                var message = await petResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            }
            else
            {
                pets = petResponse.Response;
            }

            loading = false;
        }

        private async Task GenerateInvoice()
        {
            var httpActionResponse = await Repository.Get<UserDetailDTO>($"/api/accounts/favorinue/{UserId}");
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            await LoginService.ReportUserDtAsync(httpActionResponse.Response!);
        }

        //private async Task LoadUserDetalleAsync()
        //{
        //    var responseHppt = await Repository.Get<List<User>>("/api/accounts/usuarios");
        //    if (responseHppt.Error)
        //    {
        //        var message = await responseHppt.GetErrorMessageAsync();
        //        await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
        //        return;
        //    }
        //    user = responseHppt.Response!.FirstOrDefault(u => u.Id == UserId);
        //    if (user == null)
        //    {
        //        await SweetAlertService.FireAsync("Error", "Usuario no encontrado", SweetAlertIcon.Error);
        //    }
        //}
        //private async Task LoadAsync()
        //{
        //    var responseHppt = await Repository.Get<List<Pet>>($"/api/accounts/favoritos/{UserId}");
        //    if (responseHppt.Error)
        //    {
        //        var message = await responseHppt.GetErrorMessageAsync();
        //        await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
        //        return;
        //    }
        //    pets = responseHppt.Response;
        //}
    }
}