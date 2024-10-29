namespace Zonosis.Mobile.DTOs
{
    public class UserDTO
    {
        public string FirstName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public UserType UserType { get; set; }
    }
}
