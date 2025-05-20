using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionRH.Data;
using GestionRH.Models.DTOModels;
using GestionRH.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GestionRH.Controllers.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CandidatsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CandidatsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CandidatDto>>> GetCandidats()
        {
            var candidats = await _context.Candidats
                .Select(c => new CandidatDto
                {
                    Id = c.Id,
                    Nom = c.Nom,
                    Email = c.Email,
                    Statut = c.Statut
                })
                .ToListAsync();

            return Ok(candidats);
        }

        // GET: api/CandidatsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CandidatDetailDto>> GetCandidat(int id)
        {
            var candidat = await _context.Candidats
                .Include(c => c.Entretiens)
                    .ThenInclude(e => e.Utilisateur)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidat == null)
                return NotFound();

            var dto = new CandidatDetailDto
            {
                Id = candidat.Id,
                Nom = candidat.Nom,
                Email = candidat.Email,
                CV = candidat.CV,
                Statut = candidat.Statut,
                Entretiens = candidat.Entretiens.Select(e => new EntretienDto
                {
                    Id = e.Id,
                    Date = e.Date,
                    Lieu = e.Lieu,
                    Resultat = e.Resultat,
                    UtilisateurNom = e.Utilisateur?.UserName
                }).ToList()
            };

            return Ok(dto);
        }

        // POST: api/CandidatsApi
        [HttpPost]
        public async Task<ActionResult<CandidatDto>> CreateCandidat([FromBody] CandidatDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var candidat = new Candidat
            {
                Nom = dto.Nom,
                Email = dto.Email,
                Statut = dto.Statut ?? "En attente"
                // CV géré via upload à part (API dédiée)
            };

            _context.Candidats.Add(candidat);
            await _context.SaveChangesAsync();

            dto.Id = candidat.Id;

            return CreatedAtAction(nameof(GetCandidat), new { id = candidat.Id }, dto);
        }

        // PUT: api/CandidatsApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCandidat(int id, [FromBody] CandidatDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var candidat = await _context.Candidats.FindAsync(id);
            if (candidat == null)
                return NotFound();

            candidat.Nom = dto.Nom;
            candidat.Email = dto.Email;
            candidat.Statut = dto.Statut;

            _context.Candidats.Update(candidat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/CandidatsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCandidat(int id)
        {
            var candidat = await _context.Candidats.FindAsync(id);
            if (candidat == null)
                return NotFound();

            _context.Candidats.Remove(candidat);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

