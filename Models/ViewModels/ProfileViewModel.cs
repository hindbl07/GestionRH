using System.ComponentModel.DataAnnotations;

namespace GestionRH.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Nom complet")]
        public string Nom { get; set; }

        [Phone]
        [Display(Name = "Numéro de téléphone")]
        public string PhoneNumber { get; set; }
    }
}
