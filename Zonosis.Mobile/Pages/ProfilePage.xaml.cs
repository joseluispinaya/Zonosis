namespace Zonosis.Mobile.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _viewModel;

    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void ProfileOptionRow_Tapped(object sender, string optionText)
    {
        switch (optionText)
        {
            case "Mi Adopciones":
                await _viewModel.GoToAsync(nameof(AdoptionsPage));
                //await _viewModel.ShowToastAsync("Tapp Mi Adopciones");
                break;
            case "Cambiar Clave":
                await _viewModel.ChangePasswordCommand.ExecuteAsync(null);
                //await _viewModel.ShowToastAsync("Tapp Cambiar Clave");
                break;
        }
    }
}