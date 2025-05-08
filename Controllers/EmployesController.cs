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
    public class EmployesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employes.Include(e => e.Departement).Include(e => e.Poste);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employe = await _context.Employes
                .Include(e => e.Departement)
                .Include(e => e.Poste)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employe == null)
            {
                return NotFound();
            }

            return View(employe);
        }

        // GET: Employes/Create
        public IActionResult Create()
        {
            var departements = _context.Departements.ToList();
            var postes = _context.Postes.ToList();

            // DEBUG : Afficher les éléments récupérés
            Console.WriteLine($"[DEBUG] Nombre de départements : {departements.Count}");
            Console.WriteLine($"[DEBUG] Nombre de postes : {postes.Count}");

            ViewData["DepartementId"] = new SelectList(_context.Departements, "Id", "Code");
            ViewData["PosteId"] = new SelectList(_context.Postes, "Id", "Titre");
            return View();
        }

        // POST: Employes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nom,Matricule,EmailPro,DateEmbauche,DepartementId,PosteId")] Employe employe)
        {

            Console.WriteLine("Données reçues : ");
            Console.WriteLine("Nom : " + employe.Nom);
            Console.WriteLine("PosteId : " + employe.PosteId);
            Console.WriteLine("DepartementId : " + employe.DepartementId);
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(employe.Matricule))
                {
                    employe.Matricule = await GenererMatriculeAsync();
                }

                Console.WriteLine("Matricule généré : " + employe.Matricule);  // Assure-toi que le matricule est bien généré

                _context.Add(employe);
                await _context.SaveChangesAsync();
                TempData["MatriculeCree"] = employe.Matricule;
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("Error: " + error.ErrorMessage);
                }
            }

            // Si erreur, on recharge les listes pour ne pas renvoyer une vue vide
            var departements = _context.Departements.ToList();
            var postes = _context.Postes.ToList();

            Console.WriteLine($"[DEBUG] Nombre de départements (POST) : {departements.Count}");
            Console.WriteLine($"[DEBUG] Nombre de postes (POST) : {postes.Count}");

            ViewData["DepartementId"] = new SelectList(departements, "Id", "Nom", employe.DepartementId);
            ViewData["PosteId"] = new SelectList(postes, "Id", "Titre", employe.PosteId);


            return View(employe);
        }

        // GET: Employes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employe = await _context.Employes.FindAsync(id);
            if (employe == null)
            {
                return NotFound();
            }
            ViewData["DepartementId"] = new SelectList(_context.Departements, "Id", "Code", employe.DepartementId);
            ViewData["PosteId"] = new SelectList(_context.Postes, "Id", "Titre", employe.PosteId);
            return View(employe);
        }

        // POST: Employes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Matricule,EmailPro,DateEmbauche,DepartementId,PosteId")] Employe employe)
        {
            if (id != employe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeExists(employe.Id))
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
            ViewData["DepartementId"] = new SelectList(_context.Departements, "Id", "Code", employe.DepartementId);
            ViewData["PosteId"] = new SelectList(_context.Postes, "Id", "Titre", employe.PosteId);
            return View(employe);
        }

        // GET: Employes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employe = await _context.Employes
                .Include(e => e.Departement)
                .Include(e => e.Poste)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employe == null)
            {
                return NotFound();
            }

            return View(employe);
        }

        // POST: Employes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employe = await _context.Employes.FindAsync(id);
            if (employe != null)
            {
                _context.Employes.Remove(employe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private async Task<string> GenererMatriculeAsync()
        {
            int dernierId = await _context.Employes.CountAsync(); // ou _context.Employees.MaxAsync(e => e.Id);
            int prochainNumero = dernierId + 1;

            string matricule = $"EMP{prochainNumero.ToString("D3")}"; // ← ici on définit bien la variable

            Console.WriteLine("Matricule généré : " + matricule); // maintenant c'est OK

            return matricule;
        }
        private bool EmployeExists(int id)
        {
            return _context.Employes.Any(e => e.Id == id);
        }
    }
}
