namespace Zonosis.Mobile.Pages;

public partial class AdoptionsPage : ContentPage
{
    private readonly MyAdoptionsViewModel _viewModel;

    public AdoptionsPage(MyAdoptionsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}