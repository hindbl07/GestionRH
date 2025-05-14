using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionRH.Models
{
    public class Departement
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le code est obligatoire.")]
        public string Code { get; set; }

        public ICollection<Employe> Employes { get; set; } = new List<Employe>();


        // 🔥 Cette propriété est nécessaire pour le .Include(d => d.Postes)
        public ICollection<Poste> Postes { get; set; } = new List<Poste>();
    }
}
