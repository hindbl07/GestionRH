using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionRH.Models
{
    public class Entretien
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La date de l'entretien est obligatoire.")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        public string Lieu { get; set; }

        public string Resultat { get; set; }

        [Required(ErrorMessage = "Le candidat est obligatoire.")]
        public int CandidatId { get; set; }

        public Candidat Candidat { get; set; }

        [Required(ErrorMessage = "L'utilisateur (planificateur) est obligatoire.")]
        public string UtilisateurId { get; set; }

        public ApplicationUser Utilisateur { get; set; }
    }
}
