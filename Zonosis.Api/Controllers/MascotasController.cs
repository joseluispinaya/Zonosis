using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zonosis.Api.Data;
using Zonosis.Shared.DTOs;

namespace Zonosis.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MascotasController : ControllerBase
    {
        private readonly DataContext _context;
        public MascotasController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetAsync()
        {
            var pets = await _context.Pets
                            .OrderByDescending(p => p.Id)
                            .ToListAsync();
            return Ok(pets);
        }

        [HttpGet("new/{count:int}")]
        public async Task<IActionResult> GetNewlyAddedPetsAsync(int count)
        {
            var pets = await _context.Pets
                            .OrderByDescending(p => p.Id)
                            .Take(count)
                            .ToListAsync();
            return Ok(pets);
        }

        [HttpGet("popular/{count:int}")]
        public async Task<IActionResult> GetPopularPetsAsync(int count)
        {
            var pets = await _context.Pets
                            .OrderByDescending(p => p.Views)
                            .Take(count)
                            .ToListAsync();
            return Ok(pets);
        }

        [HttpGet("random/{count:int}")]
        public async Task<IActionResult> GetRandomPetsAsync(int count)
        {
            var pets = await _context.Pets
                            .OrderByDescending(_ => Guid.NewGuid())
                            .Take(count)
                            .ToListAsync();
            return Ok(pets);
        }

        [HttpGet("{petId:int}")]
        public async Task<IActionResult> GetPetDetailsAsync(int petId)
        {
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

            return Ok(petDto);
        }
    }
}
