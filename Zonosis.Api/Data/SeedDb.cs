using Zonosis.Api.Helpers;
using Zonosis.Shared.Entities;
using Zonosis.Shared.Enumerations;

namespace Zonosis.Api.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            await CheckUserAsync("Jose", "joseluis@yopmail.com", "73999726", UserType.Admin);
            await CheckPetsAsync();
        }

        private async Task CheckPetsAsync()
        {
            if (!_context.Pets.Any())
            {
                _context.Pets.Add(new Pet
                {
                    Name = "Buddy",
                    Image = "https://firebasestorage.googleapis.com/v0/b/feriaemi-6e19d.appspot.com/o/pets%2Fimg_1.jpg?alt=media&token=1a971794-9bc1-4aca-a7ca-3315692b7875",
                    Raza = "Perro Pitbul",
                    Genero = Genero.Masculino,
                    Price = 200,
                    DateNacido = new DateTime(2021, 05, 15),
                    Description = "Perro amigable y obediente",
                    AdoptionStatus = AdoptionStatus.Disponible,
                    IsActive = true
                });
                _context.Pets.Add(new Pet
                {
                    Name = "Luna",
                    Image = "https://firebasestorage.googleapis.com/v0/b/feriaemi-6e19d.appspot.com/o/mascotasp%2Fed2134d275dc40b5a38e9dd95396c78e.jpg?alt=media&token=93232f8c-d9ed-4cae-adf7-cdd5e3312588",
                    Raza = "Gato Persa",
                    Genero = Genero.Femenino,
                    Price = 100,
                    DateNacido = new DateTime(2022, 06, 15),
                    Description = "Gato muy jugueton",
                    AdoptionStatus = AdoptionStatus.Disponible,
                    IsActive = true
                });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task<User> CheckUserAsync(string firstName, string email, string phone, UserType userType)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {

                user = new User
                {
                    FirstName = firstName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    UserType = userType,
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());
            }

            return user;
        }
    }
}
