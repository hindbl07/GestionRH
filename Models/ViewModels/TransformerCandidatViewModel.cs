using System.ComponentModel.DataAnnotations;

namespace GestionRH.Models.ViewModels
{
    public class TransformerCandidatViewModel
    {
        public int CandidatId { get; set; } 

        [Required(ErrorMessage = "Le nom est requis.")]
        public string Nom { get; set; } 

        [Required(ErrorMessage = "L'email est requis.")]
        [EmailAddress(ErrorMessage = "Adresse email invalide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le poste est requis.")]
        public int? PosteId { get; set; }

        [Required(ErrorMessage = "Le département est requis.")]
        public int? DepartementId { get; set; }

        public string Matricule { get; set; } = string.Empty;
    }
}
