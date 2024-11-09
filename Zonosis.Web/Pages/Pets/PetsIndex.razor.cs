using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Zonosis.Shared.Entities;
using Zonosis.Web.Repositories;

namespace Zonosis.Web.Pages.Pets;

public partial class PetsIndex
{
    private int currentPage = 1;
    private int totalPages;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string Page { get; set; } = string.Empty;
    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;
    [Parameter, SupplyParameterFromQuery] public int RecordsNumber { get; set; } = 10;

    public List<Pet>? Pets { get; set; }
    //private bool loading;

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
        //var responseHppt = await Repository.Get<List<Pet>>("/api/pets");
        //if (responseHppt.Error)
        //{
        //    var message = await responseHppt.GetErrorMessageAsync();
        //    await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
        //    return;
        //}
        //Pets = responseHppt.Response;
    }

    private async Task SelectedRecordsNumberAsync(int recordsnumber)
    {
        RecordsNumber = recordsnumber;
        int page = 1;
        await LoadAsync(page);
        await SelectedPageAsync(page);
    }
    private async Task FilterCallBack(string filter)
    {
        Filter = filter;
        await ApplyFilterAsync();
        StateHasChanged();
    }

    private async Task SelectedPageAsync(int page)
    {
        if (!string.IsNullOrWhiteSpace(Page))
        {
            page = Convert.ToInt32(Page);
        }

        currentPage = page;
        await LoadAsync(page);
    }

    private async Task LoadAsync(int page = 1)
    {
        var ok = await LoadListAsync(page);
        if (ok)
        {
            await LoadPagesAsync();
        }
    }

    private async Task<bool> LoadListAsync(int page)
    {
        ValidateRecordsNumber();
        var url = $"api/pets/paginated?page={page}&recordsnumber={RecordsNumber}";
        if (!string.IsNullOrEmpty(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.Get<List<Pet>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            return false;
        }
        Pets = responseHttp.Response;
        return true;
    }

    private async Task LoadPagesAsync()
    {
        ValidateRecordsNumber();
        var url = $"api/pets/totalPages?recordsnumber={RecordsNumber}";
        if (!string.IsNullOrEmpty(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.Get<int>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            return;
        }
        totalPages = responseHttp.Response;
    }

    private void ValidateRecordsNumber()
    {
        if (RecordsNumber == 0)
        {
            RecordsNumber = 10;
        }
    }

    private async Task ApplyFilterAsync()
    {
        int page = 1;
        await LoadAsync(page);
        await SelectedPageAsync(page);
    }
}