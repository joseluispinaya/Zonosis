namespace Zonosis.Mobile.ViewModels
{
    [QueryProperty(nameof(IsFirstTime), nameof(IsFirstTime))]
    public partial class LoginRegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        public LoginRegisterViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [ObservableProperty]
        private bool _isRegistrationMode;

        [ObservableProperty]
        private LoginRegisterModel _model = new();

        [ObservableProperty]
        private bool _isFirstTime;

        partial void OnIsFirstTimeChanging(bool value)
        {
            if (value)
                IsRegistrationMode = true;
        }

        [RelayCommand]
        private void ToggleMode() => IsRegistrationMode = !IsRegistrationMode;

        [RelayCommand]
        private async Task SkipForNow() => await GoToAsync($"//{nameof(HomePage)}");

        [RelayCommand]
        private async Task Submit()
        {
            if (!Model.Validate(IsRegistrationMode))
            {
                await ShowToastAsync("Todos los campos son obligatorios");
                return;
            }

            IsBusy = true;
            // Llama a LoginRegisterAsync y maneja el resultado
            var (isSuccess, errorMessage) = await _authService.LoginRegisterIaAsync(Model);
            if (isSuccess)
            {
                await SkipForNow();
            }
            else if (!string.IsNullOrEmpty(errorMessage))
            {
                await ShowAlertAsync("Error", errorMessage);
            }
            IsBusy = false;
            //llamo a la api para login o registro
            //var status = await LoginRegisterAsync(Model);
            //if (status)
            //{
            //    await SkipForNow();
            //}
            //await Task.Delay(1000);
            //await SkipForNow();


        }

        //public async Task<bool> LoginRegisterAsync(LoginRegisterModel model)
        //{
        //    bool retorna = false;
        //    string url = "https://zonosisapi.azurewebsites.net/";
        //    if (model.IsNewUser)
        //    {
        //        UserDTO userDTO = new()
        //        {
        //            Email = model.Email!,
        //            FirstName = model.Name!,
        //            Password = model.Password!,
        //            PhoneNumber = model.Phone!,
        //            UserType = UserType.User
        //        };

        //        var responseHttp = await _repository.Post<UserDTO, TokenDTO>(url, "/api/accounts/CreateUser", userDTO);

        //        if (responseHttp.Error)
        //        {
        //            var message = await responseHttp.GetErrorMessageAsync();
        //            await ShowAlertAsync("Error", message!);
        //            return false;
        //        }
        //        retorna = true;
        //    }
        //    else
        //    {
        //        LoginDTO loginDTO = new()
        //        {
        //            Email = model.Email!,
        //            Password = model.Password!,
        //        };

        //        var responseHttp = await _repository.Post<LoginDTO, TokenDTO>(url, "/api/accounts/Login", loginDTO);

        //        if (responseHttp.Error)
        //        {
        //            var message = await responseHttp.GetErrorMessageAsync();
        //            await ShowAlertAsync("Error", message!);
        //            return false;
        //        }
        //        retorna = true;
        //    }

        //    return retorna;

        //}
    }
}
