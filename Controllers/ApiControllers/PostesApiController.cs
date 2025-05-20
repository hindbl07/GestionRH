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
    public class PostesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PostesApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PosteDto>>> GetPostes()
        {
            var postes = await _context.Postes
                .Include(p => p.Departement)
                .Select(p => new PosteDto
                {
                    Id = p.Id,
                    Titre = p.Titre,
                    Description = p.Description,
                    SalaireBase = p.SalaireBase,
                    DepartementId = p.DepartementId,
                    DepartementNom = p.Departement != null ? p.Departement.Nom : null
                })
                .ToListAsync();

            return Ok(postes);
        }

        // GET: api/PostesApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PosteDto>> GetPoste(int id)
        {
            var poste = await _context.Postes
                .Include(p => p.Departement)
                .Where(p => p.Id == id)
                .Select(p => new PosteDto
                {
                    Id = p.Id,
                    Titre = p.Titre,
                    Description = p.Description,
                    SalaireBase = p.SalaireBase,
                    DepartementId = p.DepartementId,
                    DepartementNom = p.Departement != null ? p.Departement.Nom : null
                })
                .FirstOrDefaultAsync();

            if (poste == null)
                return NotFound();

            return Ok(poste);
        }

        // POST: api/PostesApi
        [HttpPost]
        public async Task<ActionResult<PosteDto>> CreatePoste([FromBody] PosteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var poste = new Poste
            {
                Titre = dto.Titre,
                Description = dto.Description,
                SalaireBase = dto.SalaireBase,
                DepartementId = dto.DepartementId
            };

            _context.Postes.Add(poste);
            await _context.SaveChangesAsync();

            dto.Id = poste.Id;

            return CreatedAtAction(nameof(GetPoste), new { id = poste.Id }, dto);
        }

        // PUT: api/PostesApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePoste(int id, [FromBody] PosteDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var poste = await _context.Postes.FindAsync(id);
            if (poste == null)
                return NotFound();

            poste.Titre = dto.Titre;
            poste.Description = dto.Description;
            poste.SalaireBase = dto.SalaireBase;
            poste.DepartementId = dto.DepartementId;

            _context.Postes.Update(poste);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/PostesApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePoste(int id)
        {
            var poste = await _context.Postes.FindAsync(id);
            if (poste == null)
                return NotFound();

            _context.Postes.Remove(poste);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
