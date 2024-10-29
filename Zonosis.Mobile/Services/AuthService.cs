using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zonosis.Mobile.Services
{
    public class AuthService
    {
        private readonly CommonService _commonService;
        private readonly IRepository _repository;
        private const string BaseUrl = "https://zonosisapi.azurewebsites.net/";
        public AuthService(CommonService commonService, IRepository repository)
        {
            _commonService = commonService;
            _repository = repository;
        }

        public async Task<(bool isSuccess, string? errorMessage)> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var userInfo = GetUser();
                var responseHttp = await _repository.PostModiAsync(BaseUrl, "/api/accounts/changePassword", changePasswordDTO, "bearer", userInfo.Token);

                if (responseHttp.Error)
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    return (false, message);
                }

                // Si no hay error
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message); // Captura cualquier excepción y retorna su mensaje
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> LoginRegisterIaAsync(LoginRegisterModel model)
        {
            var tokenApi = model.IsNewUser
                ? await RegisterUserAsync(model)
                : await LoginUserAsync(model);

            if (string.IsNullOrEmpty(tokenApi))
                return (false, "Error en el proceso de autenticación");

            var userResponse = await GetUserByTokenAsync(tokenApi);
            if (userResponse == null)
                return (false, "Error obteniendo datos del usuario");

            var user = new LoggedInUser(userResponse.Id!, userResponse.FirstName!, tokenApi);
            SetUser(user);
            _commonService.SetToken(tokenApi);
            _commonService.ToggleLoginStatus();

            return (true, null);
        }

        private async Task<string?> RegisterUserAsync(LoginRegisterModel model)
        {
            var userDTO = new UserDTO
            {
                Email = model.Email!,
                UserName = model.Email!,
                FirstName = model.Name!,
                Password = model.Password!,
                PhoneNumber = model.Phone!,
                UserType = UserType.User
            };

            var response = await _repository.Post<UserDTO, TokenDTO>(BaseUrl, "/api/accounts/CreateUser", userDTO);
            return await GetTokenOrError(response);
        }

        private async Task<string?> LoginUserAsync(LoginRegisterModel model)
        {
            var loginDTO = new LoginDTO
            {
                Email = model.Email!,
                Password = model.Password!,
            };

            var response = await _repository.Post<LoginDTO, TokenDTO>(BaseUrl, "/api/accounts/Login", loginDTO);
            return await GetTokenOrError(response);
        }

        private async Task<string?> GetTokenOrError(HttpResponseWrapper<TokenDTO> response)
        {
            if (response.Error)
            {
                var message = await response.GetErrorMessageAsync();
                await App.Current!.MainPage!.DisplayAlert("Error", message, "Ok");
                return null;
            }
            return response.Response?.Token;
        }

        private async Task<UserResponse?> GetUserByTokenAsync(string tokenApi)
        {
            var response = await _repository.GetUserByEmail<UserResponse>(BaseUrl, "/api/accounts/Getuser", "bearer", tokenApi);
            if (response.Error)
            {
                var message = await response.GetErrorMessageAsync();
                await App.Current!.MainPage!.DisplayAlert("Error", message, "Ok");
                return null;
            }
            return response.Response;
        }

        private void SetUser(LoggedInUser user) => Preferences.Default.Set(UIConstants.UserInfo, user.ToJson());

        public void Logout()
        {
            _commonService.SetToken(null);
            Preferences.Default.Remove(UIConstants.UserInfo);
            _commonService.ToggleLoginStatus();
            //_commonService.ToggleLoginStatus();
        }

        public LoggedInUser GetUser()
        {
            var userJson = Preferences.Default.Get(UIConstants.UserInfo, string.Empty);
            return LoggedInUser.LoadFromJson(userJson)!;
        }

        public bool IsLoggedIn => Preferences.Default.ContainsKey(UIConstants.UserInfo);
    }
}
