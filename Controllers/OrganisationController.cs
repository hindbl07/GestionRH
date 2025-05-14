using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionRH.Data;
using GestionRH.Models;
using System.Linq;
using GestionRH.Models.ViewModels;

namespace GestionRH.Controllers
{
    public class OrganisationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrganisationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Hierarchie()
        {
            var model = _context.Departements
                .Include(d => d.Postes)
                    .ThenInclude(p => p.Employes)
                .Select(d => new DepartementViewModel
                {
                    Nom = d.Nom,
                    Postes = d.Postes.Select(p => new PosteViewModel
                    {
                        Titre = p.Titre,
                        Employes = p.Employes.Select(e => new EmployeViewModel
                        {
                            NomComplet = e.Nom
                        }).ToList()
                    }).ToList()
                }).ToList();

            var hierarchieViewModel = new HierarchieViewModel
            {
                Departements = model
            };
            return View(hierarchieViewModel);
     
        
        }
    }
}


