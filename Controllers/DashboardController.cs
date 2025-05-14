using GestionRH.Data;
using GestionRH.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GestionRH.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Nombre total d'employés
            var totalEmployees = await _context.Employes.CountAsync();

            // Nombre total de départements
            var totalDepartments = await _context.Departements.CountAsync();

            // Nombre total de postes
            var totalPositions = await _context.Postes.CountAsync();

            // Nombre total de candidats
            var totalCandidates = await _context.Candidats.CountAsync();

            // Nombre total de recrutés (les candidats acceptés et transformés en employés)
            var totalRecruited = await _context.Employes.CountAsync(e => e.Id != null);

            // Regrouper toutes les statistiques dans un modèle
            var dashboardStats = new DashboardStats
            {
                TotalEmployees = totalEmployees,
                TotalDepartments = totalDepartments,
                TotalPositions = totalPositions,
                TotalCandidates = totalCandidates,
                TotalRecruited = totalRecruited
            };

            return View(dashboardStats);
        }
    }

    // Modèle pour stocker les statistiques du dashboard
    public class DashboardStats
    {
        public int TotalEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalPositions { get; set; }
        public int TotalCandidates { get; set; }
        public int TotalRecruited { get; set; }
    }
}
