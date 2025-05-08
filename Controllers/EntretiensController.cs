using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionRH.Data;
using GestionRH.Models;

namespace GestionRH.Controllers
{
    public class EntretiensController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EntretiensController(ApplicationDbContext context)
        {
            _context = context;
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
        public IActionResult Create()
        {
            ViewData["CandidatId"] = new SelectList(_context.Candidats, "Id", "Email");
            ViewData["UtilisateurId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Entretiens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Lieu,Resultat,CandidatId,UtilisateurId")] Entretien entretien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entretien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CandidatId"] = new SelectList(_context.Candidats, "Id", "Email", entretien.CandidatId);
            ViewData["UtilisateurId"] = new SelectList(_context.Users, "Id", "Id", entretien.UtilisateurId);
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
            ViewData["UtilisateurId"] = new SelectList(_context.Users, "Id", "Id", entretien.UtilisateurId);
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
                    _context.Update(entretien);
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
            ViewData["CandidatId"] = new SelectList(_context.Candidats, "Id", "Email", entretien.CandidatId);
            ViewData["UtilisateurId"] = new SelectList(_context.Users, "Id", "Id", entretien.UtilisateurId);
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
