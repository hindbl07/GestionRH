// Relations: Collection utilisée pour la relation 1 à plusieurs, ontient tous les entretiens planifiés par cet utilisateur.
//ICollection<T> est utilisée pour modéliser les relations "plusieurs" (one-to-many, many-to-many).
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionRH.Models
{
    public enum RoleUtilisateur
    {
        Admin,
        User
    }

    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le rôle est obligatoire.")]
        public RoleUtilisateur Role { get; set; }

        // Navigation property - entretiens planifiés par cet utilisateur RH
        public ICollection<Entretien> EntretiensPlanifies { get; set; } = new List<Entretien>();
    }
}
