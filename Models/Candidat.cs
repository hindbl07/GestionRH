using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionRH.Models
{
    public class Candidat
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide.")]
        public string Email { get; set; } 

        public string CV { get; set; }  // Chemin ou base64

        public string Statut { get; set; } = "En attente"; // Statut du candidat (En attente, Accepté, Refusé)

        // FK optionnelle (deviendra employé)
        public int? EmployeId { get; set; }
        public Employe? Employe { get; set; }

        public ICollection<Entretien> Entretiens { get; set; } = new List<Entretien>();
    }


}
