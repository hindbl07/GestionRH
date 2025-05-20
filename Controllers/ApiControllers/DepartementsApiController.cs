using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionRH.Data;
using GestionRH.Models;
using GestionRH.Models.DTOModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionRH.Controllers.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartementsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DepartementsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DepartementsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartementDto>>> GetDepartements()
        {
            var departements = await _context.Departements
                .Select(d => new DepartementDto
                {
                    Id = d.Id,
                    Nom = d.Nom,
                    Code = d.Code
                })
                .ToListAsync();

            return Ok(departements);
        }

        // GET: api/DepartementsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartementDto>> GetDepartement(int id)
        {
            var departement = await _context.Departements
                .Where(d => d.Id == id)
                .Select(d => new DepartementDto
                {
                    Id = d.Id,
                    Nom = d.Nom,
                    Code = d.Code
                })
                .FirstOrDefaultAsync();

            if (departement == null)
                return NotFound();

            return Ok(departement);
        }

        // POST: api/DepartementsApi
        [HttpPost]
        public async Task<ActionResult<DepartementDto>> CreateDepartement([FromBody] DepartementDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var departement = new Departement
            {
                Nom = dto.Nom,
                Code = dto.Code
            };

            _context.Departements.Add(departement);
            await _context.SaveChangesAsync();

            dto.Id = departement.Id;

            return CreatedAtAction(nameof(GetDepartement), new { id = departement.Id }, dto);
        }

        // PUT: api/DepartementsApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartement(int id, [FromBody] DepartementDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var departement = await _context.Departements.FindAsync(id);
            if (departement == null)
                return NotFound();

            departement.Nom = dto.Nom;
            departement.Code = dto.Code;

            _context.Departements.Update(departement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/DepartementsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartement(int id)
        {
            var departement = await _context.Departements.FindAsync(id);
            if (departement == null)
                return NotFound();

            _context.Departements.Remove(departement);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
