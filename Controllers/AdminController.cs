using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GestionRH.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using GestionRH.Models.ViewModels;

namespace GestionRH.Controllers
{
    [Authorize] // Seul un admin peut accéder à ce contrôleur
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Liste des utilisateurs
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // Détails d'un utilisateur
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Crée un ViewModel pour passer à la vue
            var model = new EditRoleViewModel
            {
                Role = user.Role
            };
            return View(model);


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(string id, RoleUtilisateur role) // Recevez RoleUtilisateur directement
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Mettre à jour le rôle de l'utilisateur
            user.Role = role; // Assurez-vous d'affecter l'énumération RoleUtilisateur

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            // Retourner la vue avec les rôles disponibles si une erreur s'est produite
            var model = new EditRoleViewModel
            {
                Role = user.Role, // Assurez-vous que Role est correctement réinitialisé
                Roles = Enum.GetValues(typeof(RoleUtilisateur))
                    .Cast<RoleUtilisateur>()
                    .Select(r => new SelectListItem
                    {
                        Value = r.ToString(),
                        Text = r.ToString()
                    }).ToList()
            };

            return View(model);
        }

        // Supprimer un utilisateur
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }
    }
}
