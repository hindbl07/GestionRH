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
    public class PostesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Postes
        public async Task<IActionResult> Index()
        {
            var postes = await _context.Postes.ToListAsync();
            Console.WriteLine("Chaîne de connexion utilisée : " + _context.Database.GetDbConnection().ConnectionString);
            Console.WriteLine($"Il y a {postes.Count} postes dans la base.");
            return View(postes);
        }

        // GET: Postes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poste = await _context.Postes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (poste == null)
            {
                return NotFound();
            }

            return View(poste);
        }

        // GET: Postes/Create
        public IActionResult Create()
        {
            Console.WriteLine(">>> Affichage du formulaire Create");
            return View();
        }

        // POST: Postes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titre,Description,SalaireBase")] Poste poste)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(poste);
                    Console.WriteLine("Connexion à la base : " + _context.Database.GetDbConnection().ConnectionString);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Poste enregistré avec succès.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de l'enregistrement du poste : " + ex.Message);
                    Console.WriteLine("Détails de l'erreur : " + ex.InnerException?.Message);
                }
            }


            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    var key = entry.Key;
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Erreur sur '{key}': {error.ErrorMessage}");
                    }
                }
            }

            return View(poste);
        }

        // GET: Postes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poste = await _context.Postes.FindAsync(id);
            if (poste == null)
            {
                return NotFound();
            }
            return View(poste);
        }

        // POST: Postes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Description,SalaireBase")] Poste poste)
        {
            if (id != poste.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(poste);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PosteExists(poste.Id))
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
            return View(poste);
        }

        // GET: Postes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poste = await _context.Postes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (poste == null)
            {
                return NotFound();
            }

            return View(poste);
        }

        // POST: Postes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var poste = await _context.Postes.FindAsync(id);
            if (poste != null)
            {
                _context.Postes.Remove(poste);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PosteExists(int id)
        {
            return _context.Postes.Any(e => e.Id == id);
        }
    }
}
