using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GestionRH.Models;
using Microsoft.AspNetCore.Authorization;
using GestionRH.Models.ViewModels;

namespace GestionRH.Controllers
{
    [AllowAnonymous] // Rend tout ce contrôleur accessible sans être connecté par défaut
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // --- LOGIN ---

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
            return View(model);
        }

        // --- REGISTER ---

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Nom du rôle par défaut
            var defaultRole = "Admin";

            // Vérifie si le rôle "Admin" existe
            var roleExists = await _roleManager.RoleExistsAsync(defaultRole);

            // Si le rôle n'existe pas, on le crée
            if (!roleExists)
            {
                var roleCreationResult = await _roleManager.CreateAsync(new IdentityRole(defaultRole));

                if (!roleCreationResult.Succeeded)
                {
                    // Si la création du rôle échoue, ajouter les erreurs et retourner à la vue
                    foreach (var error in roleCreationResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            // Créer un nouvel utilisateur
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Nom = model.Nom
            };

            // Créer l'utilisateur avec son mot de passe
            var userCreationResult = await _userManager.CreateAsync(user, model.Password);
            if (userCreationResult.Succeeded)
            {
                // Assigner le rôle "Admin" à l'utilisateur
                var addRoleResult = await _userManager.AddToRoleAsync(user, defaultRole);
                if (!addRoleResult.Succeeded)
                {
                    // Si l'ajout du rôle échoue, ajouter les erreurs et retourner à la vue
                    foreach (var error in addRoleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                // Connecter l'utilisateur après sa création
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Si la création de l'utilisateur échoue, ajouter les erreurs et retourner à la vue
            foreach (var error in userCreationResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login");

            var model = new ProfileViewModel
            {
                Nom = user.Nom,
                PhoneNumber = user.PhoneNumber
            };

            ViewData["UserName"] = user?.Nom ?? user?.UserName;

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Met à jour les champs
            user.Nom = model.Nom;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                ViewBag.Message = "Profil mis à jour avec succès.";
                ViewData["UserName"] = user.Nom ?? user.UserName;
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }



        // --- LOGOUT ---

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
