namespace Zonosis.Mobile.Pages;

public partial class AdoptionSuccessPage : ContentPage
{
	public AdoptionSuccessPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(100);
        await image.ScaleTo(1.2, 150);
        await image.ScaleTo(0.5, 150);
        await image.ScaleTo(1.2, 150);
        await image.ScaleTo(0.5, 150);
        await image.ScaleTo(1.2, 150);
        await image.ScaleTo(0.5, 150);
        await image.ScaleTo(1, 150);
    }
    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
    }
}