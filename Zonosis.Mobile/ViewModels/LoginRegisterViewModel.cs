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
                //Model = new LoginRegisterModel();
            }
            else if (!string.IsNullOrEmpty(errorMessage))
            {
                await ShowAlertAsync("Error", errorMessage);
            }
            IsBusy = false;


        }

    }
}
