using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GestionRH.Models
{
    public class Employe
    {
        [Key]
        public int Id { get; set; }

        // Si tu veux rendre le champ Nom optionnel, enlève l'attribut Required
        // [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string Nom { get; set; } = string.Empty;

        // Matricule est déjà optionnel (pas de Required ici)
        public string Matricule { get; set; } = string.Empty;

        // EmailPro : On garde la validation de l'email
        [EmailAddress(ErrorMessage = "L'adresse email n'est pas valide.")]
        public string EmailPro { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateEmbauche { get; set; }

        // DepartementId et PosteId sont déjà optionnels (int?)
        public int? DepartementId { get; set; }

        // Relation avec Departement (pas de changement nécessaire)
        [ForeignKey("DepartementId")]
        public Departement? Departement { get; set; }  // Initialisation par défaut

        public int? PosteId { get; set; }

        // Relation avec Poste (pas de changement nécessaire)
        [ForeignKey("PosteId")]
        public Poste? Poste { get; set; }   // Initialisation par défaut
    }
}

