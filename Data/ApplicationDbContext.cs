using GestionRH.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestionRH.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    // 🔹 DbSet pour chaque entité RH
    public DbSet<Employe> Employes { get; set; }
    public DbSet<Departement> Departements { get; set; }
    public DbSet<Poste> Postes { get; set; }
    public DbSet<Candidat> Candidats { get; set; }
    public DbSet<Entretien> Entretiens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 🔄 Relation : Candidat (1) ── (0..1) Employé
        modelBuilder.Entity<Candidat>()
            .HasOne(c => c.Employe)
            .WithMany()
            .HasForeignKey(c => c.EmployeId)
            .OnDelete(DeleteBehavior.Restrict);

        // 🔄 Relation : Employé (1) ── (1) Département
        modelBuilder.Entity<Employe>()
            .HasOne(e => e.Departement)
            .WithMany(d => d.Employes)
            .HasForeignKey(e => e.DepartementId);

        // 🔄 Relation : Employé (1) ── (1) Poste
        modelBuilder.Entity<Employe>()
            .HasOne(e => e.Poste)
            .WithMany(p => p.Employes)
            .HasForeignKey(e => e.PosteId);

        // 🔄 Relation : Candidat (1) ── (0..N) Entretiens
        modelBuilder.Entity<Entretien>()
            .HasOne(e => e.Candidat)
            .WithMany(c => c.Entretiens)
            .HasForeignKey(e => e.CandidatId);

        // 🔄 Relation : Utilisateur (1) ── (0..N) Entretiens planifiés
        modelBuilder.Entity<Entretien>()
            .HasOne(e => e.Utilisateur)
            .WithMany(u => u.EntretiensPlanifies)
            .HasForeignKey(e => e.UtilisateurId);

        modelBuilder.Entity<Poste>()
        .Property(p => p.SalaireBase)
        .HasPrecision(18, 2); // 18 chiffres au total, dont 2 après la virgule
    }
}
