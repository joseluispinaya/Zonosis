namespace Zonosis.Api.Helpers
{
    public interface IFileStorage
    {
        Task<string> SubirStorage(Stream streamArchivo, string carpetaDestino, string nombreArchivo);
    }
}
