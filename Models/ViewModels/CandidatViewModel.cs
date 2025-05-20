using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GestionRH.Models.ViewModels
{
    public class CandidatViewModel
    {

        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le CV est requis.")]
        [Display(Name = "CV (PDF)")]
        public IFormFile CV { get; set; }

        public string? CVActuel { get; set; } // Pour afficher un lien vers le CV actuel


        public string? Statut { get; set; } = null;


        // Indique si on est en création ou en édition
        public bool EstCreation { get; set; }
    }
}
