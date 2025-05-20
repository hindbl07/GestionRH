using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionRH.Data;
using GestionRH.Models;
using GestionRH.Services;
using GestionRH.Models.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using GestionRH.Migrations;

namespace GestionRH.Controllers
{
    [Authorize]
    public class CandidatsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly MatriculeService _matriculeService;

        public CandidatsController(ApplicationDbContext context, MatriculeService matriculeService)
        {
            _context = context;
            _matriculeService = matriculeService;
        }

        // GET: Candidats
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Candidats.Include(c => c.Employe);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Candidats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Candidats == null)
            {
                return NotFound();
            }

            var candidat = await _context.Candidats
                .Include(c => c.Employe)
                .Include(c => c.Entretiens) // 🔸 Inclure les entretiens du candidat
                    .ThenInclude(e => e.Utilisateur)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (candidat == null)
            {
                return NotFound();
            }
            return View(candidat);

        }


        // GET: Candidats/Create
        public IActionResult Create()
        {
            ViewData["EmployeId"] = new SelectList(_context.Employes, "Id", "Matricule");
            return View();
        }

        // POST: Candidats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,Email,CV")] CandidatViewModel model)
        {
            model.EstCreation = true;

            // Validation personnalisée du CV uniquement en création
            if (model.EstCreation && (model.CV == null || model.CV.Length == 0))
            {
                ModelState.AddModelError("CV", "Le CV est requis.");
            }

            if (ModelState.IsValid)
            {
                // Créer le dossier pour les fichiers téléchargés s'il n'existe pas
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cv");
                Directory.CreateDirectory(uploadsFolder);

                // Générer un nom de fichier unique
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.CV.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Sauvegarder le fichier PDF
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.CV.CopyToAsync(stream);
                }

                // Créer le candidat avec les informations du formulaire
                var candidat = new Candidat
                {
                    Nom = model.Nom,
                    Email = model.Email,
                    CV = "uploads/cv/" + uniqueFileName, // chemin relatif depuis wwwroot
                    Statut = "En attente"
                };

                _context.Add(candidat);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Si la validation échoue
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidat = await _context.Candidats.FindAsync(id);

            if (candidat == null)
            {
                return NotFound();
            }

            var model = new CandidatViewModel
            {
                Id = candidat.Id,
                Nom = candidat.Nom,
                Email = candidat.Email,
                Statut = candidat.Statut,
                CVActuel = candidat.CV // Ex : "uploads/cv/abc.pdf"
            };

            return View(model);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CandidatViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var candidat = await _context.Candidats.FindAsync(id);
            if (candidat == null)
            {
                return NotFound();
            }

            // Mise à jour des champs texte
            candidat.Nom = model.Nom;
            candidat.Email = model.Email;
            candidat.Statut = model.Statut;

            // Si un nouveau fichier CV est envoyé
            if (model.CV != null && model.CV.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cv");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.CV.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Sauvegarder le nouveau fichier
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.CV.CopyToAsync(stream);
                }

                // Supprimer l'ancien fichier (facultatif)
                if (!string.IsNullOrEmpty(candidat.CV))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", candidat.CV.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Mise à jour du chemin du nouveau fichier
                candidat.CV = "/uploads/cv/" + uniqueFileName;
            }

            try
            {
                _context.Update(candidat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Candidats.Any(e => e.Id == model.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }






        // GET: Candidats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidat = await _context.Candidats
                .Include(c => c.Employe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (candidat == null)
            {
                return NotFound();
            }

            return View(candidat);
        }

        // POST: Candidats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var candidat = await _context.Candidats.FindAsync(id);
            if (candidat == null)
            {
                return NotFound();
            }

            _context.Candidats.Remove(candidat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        // GET: Candidats/TransformerEnEmploye/5
        // GET: Candidats/Transformer/5

        public async Task<IActionResult> Transformer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidat = await _context.Candidats
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidat == null || candidat.Statut != "Accepté")
            {
                return NotFound("Candidat non trouvé ou statut non valide.");
            }

            if (candidat.EmployeId != null)
            {
                return BadRequest("Ce candidat a déjà été transformé.");
            }


            // Créer un modèle de transformation
            var model = new TransformerCandidatViewModel
            {
                CandidatId = candidat.Id,
                Nom = candidat.Nom,
                Email = candidat.Email,
                // Aucun PosteId et DepartementId dans Candidat, donc on laisse en null
                PosteId = null,
                DepartementId = null,
            };

            // Liste des départements et des postes pour le formulaire
            ViewData["DepartementId"] = new SelectList(_context.Departements, "Id", "Nom");
            ViewData["PosteId"] = new SelectList(_context.Postes, "Id", "Titre");

            return View(model);
        }


        // POST: Candidats/Transformer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transformer(TransformerCandidatViewModel model)
        {

            Console.WriteLine(">>> POST Transformer appelé <<<");
            Console.WriteLine(">>> POST Transformer appelé <<<"); // AJOUT TEST
            TempData["PostCalled"] = "Le POST a été appelé.";

            if (ModelState.IsValid)
            {
                var candidat = await _context.Candidats.FindAsync(model.CandidatId);
                if (candidat == null || candidat.Statut != "Accepté")
                {
                    return NotFound("Candidat non trouvé ou statut non valide.");
                }

                // Créer l'employé avec les informations du candidat et du formulaire
                var employe = new Employe
                {
                    Nom = model.Nom,
                    EmailPro = model.Email,
                    PosteId = model.PosteId,
                    DepartementId = model.DepartementId,
                    DateEmbauche = DateTime.Now, // Peut être ajusté
                };

                // Générer un matricule unique
                employe.Matricule = await _matriculeService.GenererMatriculeAsync();



                // Ajouter l'employé à la base de données
                _context.Employes.Add(employe);
                await _context.SaveChangesAsync();
                // Associer l'employé créé au candidat
                candidat.EmployeId = employe.Id;

                // Mettre à jour le statut du candidat (optionnel)
                candidat.Statut = "Transformé"; // Mettre à jour le statut du candidat
                _context.Update(candidat);
                await _context.SaveChangesAsync();

                // ✅ MESSAGE DE TEST
                TempData["Message"] = "Le candidat a bien été transformé en employé.";

                return RedirectToAction("Index", "Employes");
            }

            // Recharger les données pour afficher dans le formulaire en cas d'erreur
            ViewData["DepartementId"] = new SelectList(_context.Departements, "Id", "Nom", model.DepartementId);
            ViewData["PosteId"] = new SelectList(_context.Postes, "Id", "Titre", model.PosteId);

            return View(model);
        }

        public async Task<IActionResult> ExportCsv()
        {
            var candidats = await _context.Candidats
                .Include(c => c.Employe) // Si tu veux utiliser des données liées (optionnel)
                .ToListAsync();

            var builder = new StringBuilder();
            builder.AppendLine("Nom | Email | Statut | CV | EmployeMatricule");

            foreach (var c in candidats)
            {
                var matricule = c.Employe?.Matricule ?? ""; // Affiche le matricule de l'employé s’il existe
                builder.AppendLine($"{c.Nom} | {c.Email} | {c.Statut} | {c.CV} | {matricule}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "candidats.csv");
        }

        [AllowAnonymous]
        public IActionResult VoirCV(string cvFileName)
        {
            if (string.IsNullOrEmpty(cvFileName))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cv", cvFileName);

            // Vérifie si le fichier existe avant de le servir
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            return PhysicalFile(filePath, "application/pdf");
        }

        public async Task<IActionResult> HistoriqueEntretiens(int id)
        {
            var entretiens = await _context.Entretiens
                .Include(e => e.Utilisateur)
                .Where(e => e.CandidatId == id)
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            ViewBag.CandidatNom = (await _context.Candidats.FindAsync(id))?.Nom;
            return View(entretiens);
        }

        private bool CandidatExists(int id)
        {
            return _context.Candidats.Any(e => e.Id == id);
        }
    }
}