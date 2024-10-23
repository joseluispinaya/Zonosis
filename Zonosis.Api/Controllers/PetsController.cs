using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zonosis.Api.Data;

namespace Zonosis.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly DataContext _context;
        public PetsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.Pets.ToListAsync());
        }
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
    }
}
