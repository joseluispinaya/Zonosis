using Microsoft.JSInterop;
using System.Text.Json;
using Zonosis.Shared.DTOs;
using Zonosis.Shared.Entities;

namespace Zonosis.Web.Helpers
{
    public static class IJSRuntimeExtensionMethods
    {
        public static ValueTask<object> SetLocalStorage(this IJSRuntime js, string key, string content)
        {
            return js.InvokeAsync<object>("localStorage.setItem", key, content);
        }

        public static ValueTask<object> GetLocalStorage(this IJSRuntime js, string key)
        {
            return js.InvokeAsync<object>("localStorage.getItem", key);
        }

        public static ValueTask<object> RemoveLocalStorage(this IJSRuntime js, string key)
        {
            return js.InvokeAsync<object>("localStorage.removeItem", key);
        }

        public static ValueTask Reporte(this IJSRuntime js, string texto)
        {
            return js.InvokeVoidAsync("GenerarPDF", texto);
        }
        public static ValueTask ReportePet(this IJSRuntime js, Pet pet)
        {
            var petJson = JsonSerializer.Serialize(pet);
            return js.InvokeVoidAsync("GenerarPDFPET", petJson);
        }

        public static ValueTask ReportePetDt(this IJSRuntime js, PetDetailDTO pet)
        {
            var petJson = JsonSerializer.Serialize(pet);
            return js.InvokeVoidAsync("GenerarPDFPET", petJson);
        }

        public static ValueTask ReporteUserDto(this IJSRuntime js, UserDetailDTO user)
        {
            var userJson = JsonSerializer.Serialize(user);
            return js.InvokeVoidAsync("GenerarPDFUSER", userJson);
        }
    }
}
