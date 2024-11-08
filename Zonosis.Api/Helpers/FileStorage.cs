
using Firebase.Auth;
using Firebase.Storage;

namespace Zonosis.Api.Helpers
{
    public class FileStorage : IFileStorage
    {
        public async Task<string> SubirStorage(Stream streamArchivo, string carpetaDestino, string nombreArchivo)
        {
            string email = "susidelta1@gmail.com";
            string clave = "Elzero2024";
            string ruta = "garritas-98e9e.appspot.com";
            string api_key = "AIzaSyC3cYbUZTwjG_yrPzQeQ0LAInZVvbC_LzI";

            var auth = new FirebaseAuthProvider(new FirebaseConfig(api_key));
            var a = await auth.SignInWithEmailAndPasswordAsync(email, clave);

            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                ruta,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child(carpetaDestino)
                .Child(nombreArchivo)
                .PutAsync(streamArchivo, cancellation.Token);

            var downloadURL = await task;

            return downloadURL;
        }
    }
}
