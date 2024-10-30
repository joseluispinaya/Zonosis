namespace Zonosis.Shared
{
    public interface IPetHubServer
    {
        Task ViewingThisPet(int petId);
        Task ReleaseViewingThisPet(int petId);
        Task PetAdopted(int petId);
    }
}
