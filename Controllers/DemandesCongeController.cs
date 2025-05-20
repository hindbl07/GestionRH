using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionRH.Data;
using GestionRH.Models;

namespace GestionRH.Controllers
{
    public class DemandesCongeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DemandesCongeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DemandesConge
        public async Task<IActionResult> Index()
        {
            var demandes = await _context.DemandesConge
                                         .Include(d => d.Employe)
                                         .ToListAsync();
            return View(demandes);
        }


        private async Task MettreAJourStatutEmploye(int employeId)
        {
            var employe = await _context.Employes.FindAsync(employeId);
            if (employe == null) return;

            // Vérifier s’il y a un congé approuvé en cours pour cet employé
            var aujourdHui = DateTime.Today;
            bool enConge = await _context.DemandesConge.AnyAsync(d =>
                d.EmployeId == employeId &&
                d.Statut == "Approuvée" &&
                d.DateDebut <= aujourdHui &&
                d.DateFin >= aujourdHui);

            if (enConge)
            {
                employe.Statut = "En congé";
            }
            else
            {
                // Si le contrat est terminé, on peut gérer ici (exemple)
                // if (employe.DateFinContrat < aujourdHui) employe.Statut = "Contrat terminé";
                // Sinon, l’employé est actif
                employe.Statut = "Actif";
            }

            _context.Update(employe);
            await _context.SaveChangesAsync();
        }


        // GET: DemandesConge/Create
        public IActionResult Create()
        {
            ViewData["EmployeId"] = new SelectList(_context.Employes, "Id", "Nom");
            return View();
        }

        // POST: DemandesConge/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeId,DateDebut,DateFin")] DemandeConge demande)
        {
            if (demande.DateFin < demande.DateDebut)
            {
                ModelState.AddModelError("", "La date de fin doit être après la date de début.");
            }

            var employe = await _context.Employes.FindAsync(demande.EmployeId);
            if (employe == null)
            {
                ModelState.AddModelError("", "Employé introuvable.");
            }
            else
            {
                int duree = (demande.DateFin - demande.DateDebut).Days + 1;
                if (duree > employe.SoldeConge)
                {
                    ModelState.AddModelError("", $"Solde insuffisant. Solde actuel : {employe.SoldeConge} jours.");
                }
            }

            if (ModelState.IsValid)
            {
                demande.Statut = "En attente";
                _context.Add(demande);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EmployeId"] = new SelectList(_context.Employes.Select(e => new {
                e.Id,
                NomComplet = e.Nom
            }), "Id", "NomComplet");




            return View(demande);
        }

        // GET: DemandesConge/Approuver/5
        public async Task<IActionResult> Approuver(int? id)
        {
            if (id == null)
                return NotFound();

            var demande = await _context.DemandesConge
                                        .Include(d => d.Employe)
                                        .FirstOrDefaultAsync(m => m.Id == id);

            if (demande == null)
                return NotFound();

            if (demande.Statut == "En attente")
            {
                int jours = (demande.DateFin - demande.DateDebut).Days + 1;
                if (demande.Employe.SoldeConge >= jours)
                {
                    demande.Employe.SoldeConge -= jours;
                    demande.Statut = "Approuvée";
                    _context.Update(demande);
                    _context.Update(demande.Employe);
                    await _context.SaveChangesAsync();

                    // Mise à jour du statut de l'employé
                    await MettreAJourStatutEmploye(demande.EmployeId ?? 0);
                }
                else
                {
                    TempData["Error"] = "Solde insuffisant pour approuver cette demande.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: DemandesConge/Refuser/5
        public async Task<IActionResult> Refuser(int? id)
        {
            if (id == null)
                return NotFound();

            var demande = await _context.DemandesConge.FindAsync(id);
            if (demande == null)
                return NotFound();

            if (demande.Statut == "En attente")
            {
                demande.Statut = "Refusée";
                _context.Update(demande);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Optionnel : Supprimer une demande
        // GET: DemandesConge/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var demande = await _context.DemandesConge
                                        .Include(d => d.Employe)
                                        .FirstOrDefaultAsync(m => m.Id == id);

            if (demande == null)
                return NotFound();

            return View(demande);
        }

        // POST: DemandesConge/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var demande = await _context.DemandesConge.FindAsync(id);
            if (demande != null)
            {

                int employeId = demande.EmployeId ?? 0;

                _context.DemandesConge.Remove(demande);
                await _context.SaveChangesAsync();


                // Mise à jour du statut de l'employé
                if (employeId != 0)
                    await MettreAJourStatutEmploye(employeId);
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
