using System.ComponentModel.DataAnnotations;

namespace GestionRH.Models
{
    public class RegisterViewModel
    {

        [Required]
        [Display(Name = "Nom")]
        public string Nom { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
        public string ConfirmPassword { get; set; }
    }
}
