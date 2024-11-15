using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Zonosis.Api.Data;
using Zonosis.Api.Helpers;
using Zonosis.Shared.DTOs;
using Zonosis.Shared.Entities;
using Zonosis.Shared.Responses;

namespace Zonosis.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;

        public AccountsController(IUserHelper userHelper, IConfiguration configuration, DataContext context)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _context = context;
        }

        [HttpGet("usuarios")]
        public async Task<ActionResult> GetUserFullAsync()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        [HttpGet("favoritos/{userId}")]
        public async Task<ActionResult> GetUserFavoritosAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var pets = await _context.UserFavoritess
                            .Where(uf => uf.UserId == userId)
                            .Select(uf => uf.Pet)
                            .ToListAsync();

            return Ok(pets);
        }

        [HttpGet("favorinue/{userId}")]
        public async Task<ActionResult> GetUserFavorinueAsync(string userId)
        {
            var user = await _userHelper.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }

            var pets = await _context.UserFavoritess
                    .Where(uf => uf.UserId == userId)
                    .Select(uf => uf.Pet)
                    .ToListAsync();

            var userDto = new UserDetailDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                UserType = user.UserType,
                PetDetailRelDto = pets.Select(p => new PetDetailRelDTO
                {
                    Id = p!.Id,
                    Name = p.Name,
                    Image = p.Image,
                    Raza = p.Raza,
                    Genero = p.Genero,
                    Price = p.Price,
                    DateNacido = p.DateNacido,
                    Description = p.Description,
                    Views = p.Views,
                    AdoptionStatus = p.AdoptionStatus,
                    IsActive = p.IsActive
                }).ToList()
            };
            return Ok(userDto);

        }

        [HttpGet("adoptions/{userId}")]
        public async Task<ActionResult> GetUserAdoptionsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }
            var pets = await _context.UserAdoptions
                            .Where(uf => uf.UserId == userId)
                            .Select(uf => uf.Pet)
                            .ToListAsync();
            return Ok(pets);
        }

        [HttpGet("adopciones/{userId}")]
        public async Task<ActionResult> GetUserAdopcionesAsync(string userId)
        {
            var user = await _userHelper.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }

            var pets = await _context.UserAdoptions
                        .Where(uf => uf.UserId == userId)
                        .Select(uf => uf.Pet)
                        .ToListAsync();

            var userDto = new UserDetailDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                UserType = user.UserType,
                PetDetailRelDto = pets.Select(p => new PetDetailRelDTO
                {
                    Id = p!.Id,
                    Name = p.Name,
                    Image = p.Image,
                    Raza = p.Raza,
                    Genero = p.Genero,
                    Price = p.Price,
                    DateNacido = p.DateNacido,
                    Description = p.Description,
                    Views = p.Views,
                    AdoptionStatus = p.AdoptionStatus,
                    IsActive = p.IsActive
                }).ToList()
            };
            return Ok(userDto);
        }


        [HttpGet("Getuser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetUserByEmail()
        {
            User user = await _userHelper.GetUserAsync(User.Identity!.Name!);

            if (user == null)
            {
                return NotFound();
            }

            var userRespo = new UserResponse
            {
                Email = user.Email,
                FirstName = user.FirstName,
                Id = user.Id,
                PhoneNumber = user.PhoneNumber
            };
            return Ok(userRespo);

        }

        [HttpPost("changePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ChangePasswordAsync(ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()!.Description);
            }

            return NoContent();
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser([FromBody] UserDTO model)
        {
            User user = model;

            var result = await _userHelper.AddUserAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userHelper.AddUserToRoleAsync(user, user.UserType.ToString());
                return Ok(BuildToken(user));
            }
            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO model)
        {
            var result = await _userHelper.LoginAsync(model);
            if (result.Succeeded)
            {
                var user = await _userHelper.GetUserAsync(model.Email);
                return Ok(BuildToken(user));
            }
            return BadRequest("Email o contraseña incorrectos.");
        }

        private TokenDTO BuildToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Email!),
                new(ClaimTypes.Role, user.UserType.ToString()),
                new("FirstName", user.FirstName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddDays(60);
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new TokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
