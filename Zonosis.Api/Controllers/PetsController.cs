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
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IFileStorage _fileStorage;
        public PetsController(DataContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.Pets.ToListAsync());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var pet = await _context.Pets
                .FirstOrDefaultAsync(x => x.Id == id);

            if (pet == null)
            {
                return NotFound();
            }

            return Ok(pet);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(PetDto petDto)
        {
            try
            {
                if (!string.IsNullOrEmpty(petDto.Image))
                {
                    var photoUser = Convert.FromBase64String(petDto.Image);
                    var strea = new MemoryStream(photoUser);
                    string nombrecodi = Guid.NewGuid().ToString("N");
                    var filenan = $"{nombrecodi}.jpg";
                    petDto.Image = await _fileStorage.SubirStorage(strea, "mascotasp", filenan);
                }
                Pet newPer = new()
                {
                    Name = petDto.Name,
                    Image = petDto.Image,
                    Raza = petDto.Raza,
                    //Genero = Genero.Masculino,
                    Genero = petDto.Genero,
                    Price = petDto.Price,
                    //DateNacido = DateTime.UtcNow,
                    DateNacido = petDto.DateNacido.ToUniversalTime(),
                    Description = petDto.Description,
                    AdoptionStatus = AdoptionStatus.Disponible,
                    IsActive = true
                };
                _context.Add(newPer);
                await _context.SaveChangesAsync();
                return Ok(newPer);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> PutAsync(PetDto petDto)
        {
            try
            {
                if (!string.IsNullOrEmpty(petDto.Image))
                {
                    var photoUser = Convert.FromBase64String(petDto.Image);
                    var strea = new MemoryStream(photoUser);
                    string nombrecodi = Guid.NewGuid().ToString("N");
                    var filenan = $"{nombrecodi}.jpg";
                    petDto.Image = await _fileStorage.SubirStorage(strea, "mascotasp", filenan);
                }
                var petma = await _context.Pets
                .FirstOrDefaultAsync(x => x.Id == petDto.Id);
                if (petma == null)
                {
                    return NotFound();
                }

                petma.Name = petDto.Name;
                petma.Image = !string.IsNullOrEmpty(petDto.Image) && petDto.Image != petma.Image ? petDto.Image : petma.Image;
                petma.Raza = petDto.Raza;
                petma.Genero = petDto.Genero;
                petma.Price = petDto.Price;
                petma.DateNacido = petDto.DateNacido.ToUniversalTime();
                petma.Description = petDto.Description;
                //petma.AdoptionStatus = AdoptionStatus.Disponible;
                //petma.IsActive = true;

                _context.Update(petma);
                await _context.SaveChangesAsync();
                return Ok(petma);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
