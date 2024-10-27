namespace Zonosis.Mobile.Pages;

public partial class LoginRegisterPage : ContentPage
{
    private readonly LoginRegisterViewModel _viewModel;

    public LoginRegisterPage(LoginRegisterViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}