using GestionRH.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionRH.Services
{

    public class MatriculeService
    {
        private readonly ApplicationDbContext _context;

        public MatriculeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenererMatriculeAsync()
        {
            // Compter le nombre d'employés existants pour générer le matricule
            int dernierId = await _context.Employes.CountAsync();  // CountAsync() devrait fonctionner ici

            // Générer le matricule
            int prochainNumero = dernierId + 1;
            string matricule = $"EMP{prochainNumero.ToString("D3")}";  // Matricule avec un format "EMP001", "EMP002", etc.

            return matricule;
        }
    }

}
