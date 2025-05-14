using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionRH.Data;
using GestionRH.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace GestionRH.Controllers
{
    public class EntretiensController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public EntretiensController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Entretiens
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Entretiens.Include(e => e.Candidat).Include(e => e.Utilisateur);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Entretiens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entretien = await _context.Entretiens
                .Include(e => e.Candidat)
                .Include(e => e.Utilisateur)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entretien == null)
            {
                return NotFound();
            }

            return View(entretien);
        }

        // GET: Entretiens/Create
        // GET: Entretiens/Create
        public async Task<IActionResult> Create()
        {
            // Filtrer les candidats avec statut "En attente"
            var candidatsEnAttente = await _context.Candidats
                .Where(c => c.Statut == "En attente")
                .ToListAsync();

            // On ne montre que les candidats "En attente"
            ViewData["CandidatId"] = new SelectList(
                _context.Candidats.Where(c => c.Statut == "En attente"),
                "Id", "Nom"
            );


            // Liste des utilisateurs RH (rôle = User)
            var utilisateursRH = await _userManager.Users
                .Where(u => u.Role == RoleUtilisateur.User)
                .ToListAsync();

            ViewData["UtilisateurId"] = new SelectList(utilisateursRH, "Id", "Nom");

            return View();
        }

        // POST: Entretiens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,Lieu,Resultat,CandidatId,UtilisateurId")] Entretien entretien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entretien);

                // Mettre à jour le statut du candidat si résultat = Accepté ou Refusé
                var candidat = await _context.Candidats.FindAsync(entretien.CandidatId);
                if (candidat != null)
                {
                    if (entretien.Resultat == "Accepté")
                        candidat.Statut = "Accepté";
                    else if (entretien.Resultat == "Refusé")
                        candidat.Statut = "Refusé";
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CandidatId"] = new SelectList(_context.Candidats, "Id", "Nom", entretien.CandidatId);

            var utilisateursRH = await _userManager.Users
                .Where(u => u.Role == RoleUtilisateur.User)
                .ToListAsync();
            ViewData["UtilisateurId"] = new SelectList(utilisateursRH, "Id", "Nom", entretien.UtilisateurId);

            return View(entretien);
        }



        // GET: Entretiens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entretien = await _context.Entretiens.FindAsync(id);
            if (entretien == null)
            {
                return NotFound();
            }
            ViewData["CandidatId"] = new SelectList(_context.Candidats, "Id", "Email", entretien.CandidatId);
            ViewData["UtilisateurId"] = new SelectList(_context.Users, "Id", "Nom", entretien.UtilisateurId);
            return View(entretien);
        }

        // POST: Entretiens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Lieu,Resultat,CandidatId,UtilisateurId")] Entretien entretien)
        {
            if (id != entretien.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Mettre à jour l’entretien
                    _context.Update(entretien);

                    // Mettre à jour le statut du candidat lié
                    var candidat = await _context.Candidats.FindAsync(entretien.CandidatId);
                    if (candidat != null)
                    {
                        if (entretien.Resultat == "Accepté")
                            candidat.Statut = "Accepté";
                        else if (entretien.Resultat == "Refusé")
                            candidat.Statut = "Refusé";
                        // Si "En attente" → ne pas changer le statut
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntretienExists(entretien.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Recharger les listes en cas d'erreur
            ViewData["CandidatId"] = new SelectList(_context.Candidats, "Id", "Email", entretien.CandidatId);
            ViewData["UtilisateurId"] = new SelectList(_context.Users, "Id", "Nom", entretien.UtilisateurId);
            return View(entretien);
        }

        // GET: Entretiens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entretien = await _context.Entretiens
                .Include(e => e.Candidat)
                .Include(e => e.Utilisateur)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entretien == null)
            {
                return NotFound();
            }

            return View(entretien);
        }

        // POST: Entretiens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entretien = await _context.Entretiens.FindAsync(id);
            if (entretien != null)
            {
                _context.Entretiens.Remove(entretien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EntretienExists(int id)
        {
            return _context.Entretiens.Any(e => e.Id == id);
        }
    }
}
