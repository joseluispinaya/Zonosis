using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Zonosis.Shared.Entities;
using Zonosis.Web.Repositories;

namespace Zonosis.Web.Pages.Pets;

public partial class PetsIndex
{
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

    public List<Pet>? Pets { get; set; }
    //private bool loading;

    protected override async Task OnInitializedAsync()
    {
        //loading = true;

        var responseHppt = await Repository.Get<List<Pet>>("/api/pets");
        if (responseHppt.Error)
        {
            var message = await responseHppt.GetErrorMessageAsync();
            await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            return;
        }
        Pets = responseHppt.Response;
        //loading = false;
    }
}