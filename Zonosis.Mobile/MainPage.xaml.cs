namespace Zonosis.Mobile
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Check if onboarding screen shown
            if (Preferences.Default.ContainsKey(UIConstants.OnboardingShown))
                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            else
                await Shell.Current.GoToAsync($"//{nameof(OnboardingPage)}");
        }

    }

}
