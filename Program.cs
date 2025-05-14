using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using GestionRH.Data;
using GestionRH.Models;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using GestionRH.Services;

namespace GestionRH;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Chargement de la chaîne de connexion à la base de données
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString)
                   .EnableSensitiveDataLogging()
                   .LogTo(message => Debug.WriteLine(message), LogLevel.Information)
        );

        // Ajout du système d'identité avec utilisateur personnalisé
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            // Tu peux ajouter d'autres options ici (Password, Lockout, etc.)
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        // Ajout du service MatriculeService
        builder.Services.AddScoped<MatriculeService>();  // Ligne ajoutée


        // Filtre global : tous les contrôleurs exigent une authentification
        builder.Services.AddControllersWithViews(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });

        // Configuration du cookie d’authentification
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // expiration après 30 minutes d’inactivité
            options.ReturnUrlParameter = "returnUrl";
        });

        // Razor Pages (utile si tu en utilises pour l'identité par défaut)
        builder.Services.AddRazorPages();

        var app = builder.Build();

        // Ajout de rôles au démarrage de l'application
        var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
       
        var roles = new string[] { RoleUtilisateur.Admin.ToString(), RoleUtilisateur.User.ToString() };

        foreach (var role in roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(role);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Gestion des erreurs
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        // Middleware essentiels
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();


        app.UseAuthentication();
        app.UseAuthorization();

        // Redirection automatique des codes 401/403 vers /Account/Login
        app.UseStatusCodePagesWithRedirects("/Account/Login?returnUrl={0}");

        // Routes
        // Bonne route par défaut : vers Dashboard
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Dashboard}/{action=Index}/{id?}");


        app.MapRazorPages();

        app.Run();
    }
}
