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
    public class EntretiensApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EntretiensApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EntretiensApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntretienDto>>> GetEntretiens()
        {
            var entretiens = await _context.Entretiens
                .Include(e => e.Candidat)
                .Include(e => e.Utilisateur)
                .Select(e => new EntretienDto
                {
                    Id = e.Id,
                    Date = e.Date,
                    Lieu = e.Lieu,
                    Resultat = e.Resultat,
                    CandidatId = e.CandidatId ?? 0,
                    CandidatNom = e.Candidat != null ? e.Candidat.Nom : null,
                    UtilisateurId = e.UtilisateurId,
                    UtilisateurNom = e.Utilisateur != null ? e.Utilisateur.UserName : null
                })
                .ToListAsync();

            return Ok(entretiens);
        }

        // GET: api/EntretiensApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EntretienDto>> GetEntretien(int id)
        {
            var entretien = await _context.Entretiens
                .Include(e => e.Candidat)
                .Include(e => e.Utilisateur)
                .Where(e => e.Id == id)
                .Select(e => new EntretienDto
                {
                    Id = e.Id,
                    Date = e.Date,
                    Lieu = e.Lieu,
                    Resultat = e.Resultat,
                    CandidatId = e.CandidatId ?? 0,
                    CandidatNom = e.Candidat != null ? e.Candidat.Nom : null,
                    UtilisateurId = e.UtilisateurId,
                    UtilisateurNom = e.Utilisateur != null ? e.Utilisateur.UserName : null
                })
                .FirstOrDefaultAsync();

            if (entretien == null)
                return NotFound();

            return Ok(entretien);
        }

        // POST: api/EntretiensApi
        [HttpPost]
        public async Task<ActionResult<EntretienDto>> CreateEntretien([FromBody] EntretienDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entretien = new Entretien
            {
                Date = dto.Date,
                Lieu = dto.Lieu,
                Resultat = dto.Resultat,
                CandidatId = dto.CandidatId,
                UtilisateurId = dto.UtilisateurId ?? string.Empty
            };

            _context.Entretiens.Add(entretien);

            // Mettre à jour le statut du candidat selon le résultat
            var candidat = await _context.Candidats.FindAsync(dto.CandidatId);
            if (candidat != null)
            {
                if (entretien.Resultat == "Accepté" || entretien.Resultat == "Refusé")
                {
                    candidat.Statut = entretien.Resultat;
                    _context.Candidats.Update(candidat);
                }
            }

            await _context.SaveChangesAsync();

            dto.Id = entretien.Id;

            return CreatedAtAction(nameof(GetEntretien), new { id = dto.Id }, dto);
        }

        // PUT: api/EntretiensApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEntretien(int id, [FromBody] EntretienDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var entretien = await _context.Entretiens.FindAsync(id);
            if (entretien == null)
                return NotFound();

            entretien.Date = dto.Date;
            entretien.Lieu = dto.Lieu;
            entretien.Resultat = dto.Resultat;
            entretien.CandidatId = dto.CandidatId;
            entretien.UtilisateurId = dto.UtilisateurId ?? string.Empty;

            _context.Entretiens.Update(entretien);

            // Mise à jour du statut candidat
            var candidat = await _context.Candidats.FindAsync(dto.CandidatId);
            if (candidat != null)
            {
                if (dto.Resultat == "Accepté" || dto.Resultat == "Refusé")
                {
                    candidat.Statut = dto.Resultat;
                    _context.Candidats.Update(candidat);
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/EntretiensApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntretien(int id)
        {
            var entretien = await _context.Entretiens.FindAsync(id);
            if (entretien == null)
                return NotFound();

            _context.Entretiens.Remove(entretien);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
