namespace Zonosis.Mobile.Models
{
    public partial class LoginRegisterModel : ObservableObject
    {
        [ObservableProperty]
        private string? _name;

        [ObservableProperty]
        private string? _email;

        [ObservableProperty]
        private string? _password;

        [ObservableProperty]
        private string? _phone;

        public bool IsNewUser => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Phone);
        //public bool IsNewUser => !string.IsNullOrWhiteSpace(Name);

        public bool Validate(bool isRegistrationMode)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                return false;

            if (isRegistrationMode && (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Phone)))
                return false;

            return true;
        }
    }
}
