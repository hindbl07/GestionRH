using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestionRH.Models
{
    public class Poste
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire.")]
        public string Titre { get; set; }

        [Required(ErrorMessage = "La description est obligatoire.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Le salaire est obligatoire.")]
        [Range(1000, 100000, ErrorMessage = "Le salaire doit être entre 1000 et 100000.")]
        [Precision(18, 2)]
        public decimal SalaireBase { get; set; }


        // ✅ Lien vers le Département
        public int? DepartementId { get; set; }  // plus nullable maintenant
        
        [ForeignKey("DepartementId")]
        public Departement? Departement { get; set; }

        public ICollection<Employe> Employes { get; set; } = new List<Employe>(); // Un poste peut être occupé par plusieurs employés

        public Poste() { } // constructeur vide
       
    }
}