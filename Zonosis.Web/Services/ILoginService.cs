using Zonosis.Shared.DTOs;
using Zonosis.Shared.Entities;

namespace Zonosis.Web.Services
{
    public interface ILoginService
    {
        Task LoginAsync(string token);

        Task LogoutAsync();
        Task ReportAsync(string texto);
        Task ReportPetAsync(Pet pet);
        Task ReportPetDtAsync(PetDetailDTO pet);
    }
}
