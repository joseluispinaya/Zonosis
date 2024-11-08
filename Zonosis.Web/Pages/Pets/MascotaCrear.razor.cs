using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Zonosis.Shared.DTOs;
using Zonosis.Shared.Enumerations;
using Zonosis.Web.Helpers;
using Zonosis.Web.Repositories;

namespace Zonosis.Web.Pages.Pets;

public partial class MascotaCrear
{
    private PetDto petDto = new();
    private bool loading;
    private string? imageUrl;
    private string? selectedGeneroType;

    private List<SelectorModel> selected { get; set; } = new();
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;

    protected override void OnInitialized()
    {
        if (petDto.DateNacido == DateTime.MinValue)
        {
            petDto.DateNacido = DateTime.Today;
        }
        //base.OnInitialized();
        selected = Enum.GetValues(typeof(Genero))
            .Cast<Genero>()
            .Select(g => new SelectorModel(((int)g).ToString(), g.ToString()))
            .ToList();
    }

    private async Task OnGeneroChange(ChangeEventArgs e)
    {

        selectedGeneroType = e.Value?.ToString();

        // Validación de la opción "-- Seleccione Genero --"
        if (string.IsNullOrEmpty(selectedGeneroType) || selectedGeneroType == "-1")
        {
            await SweetAlertService.FireAsync("Advertencia", "Debe seleccionar un género válido.", SweetAlertIcon.Warning);
            return;
        }

        // Intentar convertir el valor seleccionado a un valor del enum Genero
        if (Enum.TryParse<Genero>(selectedGeneroType, out Genero generoSel))
        {
            petDto.Genero = generoSel;
        }
        else
        {
            await SweetAlertService.FireAsync("Error", "No se pudo convertir el género seleccionado.", SweetAlertIcon.Error);
        }
    }
    private void ImageSelected(string imagenBase64)
    {
        petDto.Image = imagenBase64;
        imageUrl = null;
    }

    private async Task CreteUserAsync()
    {
        //petDto.Genero = 
        loading = true;
        var responseHttp = await Repository.Post("/api/pets", petDto);
        loading = false;

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            return;
        }
        Return();

        var toast = SweetAlertService.Mixin(new SweetAlertOptions
        {
            Toast = true,
            Position = SweetAlertPosition.BottomEnd,
            ShowConfirmButton = true,
            Timer = 3000
        });
        await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Registrado con Exito.");

    }

    private void Return()
    {
        NavigationManager.NavigateTo("/pets");
    }
}