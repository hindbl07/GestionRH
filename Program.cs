using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using GestionRH.Data;
using GestionRH.Models;
using System.Diagnostics;

namespace GestionRH;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Chargement de la cha�ne de connexion � la base de donn�es
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString)
                   .EnableSensitiveDataLogging()
                   .LogTo(message => Debug.WriteLine(message), LogLevel.Information)
        );

        // Ajout du syst�me d'identit� avec utilisateur personnalis�
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            // Tu peux ajouter d'autres options ici (Password, Lockout, etc.)
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        // Filtre global : tous les contr�leurs exigent une authentification
        builder.Services.AddControllersWithViews(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });

        // Configuration du cookie d�authentification
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // expiration apr�s 30 minutes d�inactivit�
            options.ReturnUrlParameter = "returnUrl";
        });

        // Razor Pages (utile si tu en utilises pour l'identit� par d�faut)
        builder.Services.AddRazorPages();

        var app = builder.Build();

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
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapRazorPages();

        app.Run();
    }
}
