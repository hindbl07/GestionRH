using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionRH.Data;
using GestionRH.Models;
using GestionRH.Models.DTOModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionRH.Controllers.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EmployesApi?search=...&departementId=...&posteId=...
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeDto>>> GetEmployes(string? search, int? departementId, int? posteId)
        {
            var query = _context.Employes
                .Include(e => e.Departement)
                .Include(e => e.Poste)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e =>
                    e.Nom.Contains(search) ||
                    e.EmailPro.Contains(search) ||
                    e.Matricule.Contains(search) ||
                    (e.Poste != null && e.Poste.Titre.Contains(search)) ||
                    (e.Departement != null && e.Departement.Nom.Contains(search))
                );
            }

            if (departementId.HasValue)
                query = query.Where(e => e.DepartementId == departementId.Value);

            if (posteId.HasValue)
                query = query.Where(e => e.PosteId == posteId.Value);

            var employes = await query
                .Select(e => new EmployeDto
                {
                    Id = e.Id,
                    Nom = e.Nom,
                    Matricule = e.Matricule,
                    EmailPro = e.EmailPro,
                    DateEmbauche = e.DateEmbauche,
                    SoldeConge = e.SoldeConge,
                    Statut = e.Statut,
                    DepartementId = e.DepartementId,
                    DepartementNom = e.Departement != null ? e.Departement.Nom : null,
                    PosteId = e.PosteId,
                    PosteTitre = e.Poste != null ? e.Poste.Titre : null
                })
                .ToListAsync();

            return Ok(employes);
        }

        // GET: api/EmployesApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeDto>> GetEmploye(int id)
        {
            var e = await _context.Employes
                .Include(emp => emp.Departement)
                .Include(emp => emp.Poste)
                .Where(emp => emp.Id == id)
                .Select(emp => new EmployeDto
                {
                    Id = emp.Id,
                    Nom = emp.Nom,
                    Matricule = emp.Matricule,
                    EmailPro = emp.EmailPro,
                    DateEmbauche = emp.DateEmbauche,
                    SoldeConge = emp.SoldeConge,
                    Statut = emp.Statut,
                    DepartementId = emp.DepartementId,
                    DepartementNom = emp.Departement != null ? emp.Departement.Nom : null,
                    PosteId = emp.PosteId,
                    PosteTitre = emp.Poste != null ? emp.Poste.Titre : null
                })
                .FirstOrDefaultAsync();

            if (e == null)
                return NotFound();

            return Ok(e);
        }

        // POST: api/EmployesApi
        [HttpPost]
        public async Task<ActionResult<EmployeDto>> CreateEmploye([FromBody] EmployeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employe = new Employe
            {
                Nom = dto.Nom,
                Matricule = string.IsNullOrEmpty(dto.Matricule) ? await GenererMatriculeAsync() : dto.Matricule,
                EmailPro = dto.EmailPro,
                DateEmbauche = dto.DateEmbauche,
                SoldeConge = dto.SoldeConge,
                Statut = dto.Statut,
                DepartementId = dto.DepartementId,
                PosteId = dto.PosteId
            };

            _context.Employes.Add(employe);
            await _context.SaveChangesAsync();

            dto.Id = employe.Id;
            dto.Matricule = employe.Matricule;

            return CreatedAtAction(nameof(GetEmploye), new { id = employe.Id }, dto);
        }

        // PUT: api/EmployesApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmploye(int id, [FromBody] EmployeDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var employe = await _context.Employes.FindAsync(id);
            if (employe == null)
                return NotFound();

            employe.Nom = dto.Nom;
            employe.EmailPro = dto.EmailPro;
            employe.DateEmbauche = dto.DateEmbauche;
            employe.SoldeConge = dto.SoldeConge;
            employe.Statut = dto.Statut;
            employe.DepartementId = dto.DepartementId;
            employe.PosteId = dto.PosteId;

            _context.Employes.Update(employe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/EmployesApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmploye(int id)
        {
            var employe = await _context.Employes.FindAsync(id);
            if (employe == null)
                return NotFound();

            // Supprimer les demandes de congé liées
            var demandes = _context.DemandesConge.Where(dc => dc.EmployeId == id);
            _context.DemandesConge.RemoveRange(demandes);

            _context.Employes.Remove(employe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<string> GenererMatriculeAsync()
        {
            int count = await _context.Employes.CountAsync();
            return $"EMP{(count + 1).ToString("D3")}";
        }
    }
}
