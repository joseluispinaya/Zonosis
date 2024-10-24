using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zonosis.Api.Data;
using Zonosis.Api.Helpers;
using Zonosis.Shared.DTOs;
using Zonosis.Shared.Entities;
using Zonosis.Shared.Enumerations;

namespace Zonosis.Api.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private static readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly IUserHelper _userHelper;
        private readonly DataContext _context;
        public UsersController(IUserHelper userHelper, DataContext context)
        {
            _userHelper = userHelper;
            _context = context;
        }

        // api/user/adopt/1
        [HttpPost("adopt/{petId:int}")]
        public async Task<ActionResult> AdoptPetAsync(int petId)
        {
            try
            {
                await _semaphore.WaitAsync();

                var pet = await _context.Pets
                    .AsTracking().FirstOrDefaultAsync(p => p.Id == petId);
                if (pet == null)
                {
                    return NotFound();
                }
                if (pet.AdoptionStatus == AdoptionStatus.Adoptado)
                {
                    return BadRequest($"{pet.Name} ya esta adoptado.");
                }
                pet.AdoptionStatus = AdoptionStatus.Adoptado;
                var user = await _userHelper.GetUserAsync(User.Identity!.Name!);

                var userAdoption = new UserAdoption
                {
                    UserId = user.Id,
                    PetId = petId
                };
                await _context.UserAdoptions.AddAsync(userAdoption);
                await _context.SaveChangesAsync();
                return Ok(pet);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        //  api/user/adoptions
        [HttpGet("adoptions")]
        public async Task<ActionResult> GetUserAdoptionsAsync()
        {
            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }
            var pets = await _context.UserAdoptions
                            .Where(uf => uf.UserId == user.Id)
                            .Select(uf => uf.Pet)
                            .ToListAsync();
            return Ok(pets);
        }

        //  api/user/favorites
        [HttpGet("favorites")]
        public async Task<ActionResult> GetUserFavoritesAsync()
        {
            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }
            var pets = await _context.UserFavoritess
                            .Where(uf => uf.UserId == user.Id)
                            .Select(uf => uf.Pet)
                            .ToListAsync();
            return Ok(pets);
        }

        //  api/user/favorites/1
        [HttpPost("favorites/{petId:int}")]
        public async Task<ActionResult> ToggleFavoritesAsync(int petId)
        {
            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }
            var userFavorite = await _context.UserFavoritess
            .FirstOrDefaultAsync(uf => uf.UserId == user.Id && uf.PetId == petId);

            if (userFavorite is not null)
            {
                _context.UserFavoritess.Remove(userFavorite);
            }
            else
            {
                userFavorite = new UserFavorites
                {
                    UserId = user.Id,
                    PetId = petId
                };
                await _context.UserFavoritess.AddAsync(userFavorite);
            }
            await _context.SaveChangesAsync();
            return Ok(userFavorite);
        }

        //  api/user/view-pet-details/1
        [HttpGet("view-pet-details/{petId:int}")]
        public async Task<IActionResult> GetPetDetailsAsync(int petId)
        {
            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }
            var petDetails = await _context.Pets
                .AsTracking()
                .FirstOrDefaultAsync(p => p.Id == petId);

            if (petDetails is not null)
            {
                petDetails.Views++;
                _context.SaveChanges();
            }

            var petDto = new PetDetailDTO
            {
                AdoptionStatus = petDetails!.AdoptionStatus,
                Raza = petDetails.Raza,
                DateNacido = petDetails.DateNacido,
                Description = petDetails.Description,
                Genero = petDetails.Genero,
                Id = petDetails.Id,
                Image = petDetails.Image,
                Name = petDetails.Name,
                Price = petDetails.Price
            };

            if (await _context.UserFavoritess.AnyAsync(uf => uf.UserId == user.Id && uf.PetId == petId))
            {
                petDto.IsFavorite = true;
            }

            return Ok(petDto);
        }
    }
}
