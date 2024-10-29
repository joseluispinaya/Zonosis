namespace Zonosis.Mobile.Pages;

public partial class DetailsPage : ContentPage
{
    private readonly DetailsViewModel _viewModel;

    public DetailsPage(DetailsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}