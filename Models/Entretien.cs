using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionRH.Models;


namespace GestionRH.Models
{
    public class Entretien
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La date de l'entretien est obligatoire.")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        public string Lieu { get; set; } = string.Empty;

        public string Resultat { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le candidat est obligatoire.")]
        public int? CandidatId { get; set; }

        [ForeignKey("CandidatId")]
        public Candidat? Candidat { get; set; }

        public string UtilisateurId { get; set; } = String.Empty;

        [ForeignKey("UtilisateurId")]
        public ApplicationUser? Utilisateur { get; set; }
    }
}