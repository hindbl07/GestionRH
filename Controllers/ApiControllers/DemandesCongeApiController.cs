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
    public class DemandesCongeApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DemandesCongeApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DemandesCongeApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DemandeCongeDto>>> GetDemandesConge()
        {
            var demandes = await _context.DemandesConge
                .Include(d => d.Employe)
                .Select(d => new DemandeCongeDto
                {
                    Id = d.Id,
                    EmployeId = d.EmployeId,
                    EmployeNom = d.Employe != null ? d.Employe.Nom : null,
                    DateDebut = d.DateDebut,
                    DateFin = d.DateFin,
                    Statut = d.Statut
                })
                .ToListAsync();

            return Ok(demandes);
        }

        // GET: api/DemandesCongeApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DemandeCongeDto>> GetDemandeConge(int id)
        {
            var demande = await _context.DemandesConge
                .Include(d => d.Employe)
                .Where(d => d.Id == id)
                .Select(d => new DemandeCongeDto
                {
                    Id = d.Id,
                    EmployeId = d.EmployeId,
                    EmployeNom = d.Employe != null ? d.Employe.Nom : null,
                    DateDebut = d.DateDebut,
                    DateFin = d.DateFin,
                    Statut = d.Statut
                })
                .FirstOrDefaultAsync();

            if (demande == null)
                return NotFound();

            return Ok(demande);
        }

        // POST: api/DemandesCongeApi
        [HttpPost]
        public async Task<ActionResult<DemandeCongeDto>> CreateDemandeConge([FromBody] DemandeCongeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Vérifications similaires à celles dans MVC peuvent être faites ici (dates, solde congé...)

            var employe = await _context.Employes.FindAsync(dto.EmployeId);
            if (employe == null)
                return BadRequest("Employé non trouvé.");

            int duree = (dto.DateFin - dto.DateDebut).Days + 1;
            if (duree <= 0)
                return BadRequest("La date de fin doit être après la date de début.");

            if (duree > employe.SoldeConge)
                return BadRequest($"Solde insuffisant. Solde actuel : {employe.SoldeConge} jours.");

            var demande = new DemandeConge
            {
                EmployeId = dto.EmployeId,
                DateDebut = dto.DateDebut,
                DateFin = dto.DateFin,
                Statut = "En attente"
            };

            _context.DemandesConge.Add(demande);
            await _context.SaveChangesAsync();

            dto.Id = demande.Id;
            dto.Statut = demande.Statut;

            return CreatedAtAction(nameof(GetDemandeConge), new { id = dto.Id }, dto);
        }

        // PUT: api/DemandesCongeApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDemandeConge(int id, [FromBody] DemandeCongeDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var demande = await _context.DemandesConge.FindAsync(id);
            if (demande == null)
                return NotFound();

            // Autoriser la mise à jour seulement si en attente (ou selon règles métier)
            if (demande.Statut != "En attente")
                return BadRequest("Seules les demandes en attente peuvent être modifiées.");

            // Mettre à jour les champs modifiables
            demande.DateDebut = dto.DateDebut;
            demande.DateFin = dto.DateFin;
            demande.EmployeId = dto.EmployeId;

            _context.DemandesConge.Update(demande);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/DemandesCongeApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDemandeConge(int id)
        {
            var demande = await _context.DemandesConge.FindAsync(id);
            if (demande == null)
                return NotFound();

            _context.DemandesConge.Remove(demande);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/DemandesCongeApi/5/approuver
        [HttpPost("{id}/approuver")]
        public async Task<IActionResult> ApprouverDemandeConge(int id)
        {
            var demande = await _context.DemandesConge
                .Include(d => d.Employe)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (demande == null)
                return NotFound();

            if (demande.Statut != "En attente")
                return BadRequest("Cette demande ne peut pas être approuvée.");

            var employe = demande.Employe;
            if (employe == null)
                return BadRequest("Employé non trouvé.");

            int duree = (demande.DateFin - demande.DateDebut).Days + 1;

            if (employe.SoldeConge < duree)
                return BadRequest("Solde insuffisant pour approuver cette demande.");

            employe.SoldeConge -= duree;
            demande.Statut = "Approuvée";

            _context.DemandesConge.Update(demande);
            _context.Employes.Update(employe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/DemandesCongeApi/5/refuser
        [HttpPost("{id}/refuser")]
        public async Task<IActionResult> RefuserDemandeConge(int id)
        {
            var demande = await _context.DemandesConge.FindAsync(id);
            if (demande == null)
                return NotFound();

            if (demande.Statut != "En attente")
                return BadRequest("Cette demande ne peut pas être refusée.");

            demande.Statut = "Refusée";
            _context.DemandesConge.Update(demande);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
