using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GestionRH.Models
{
    public class DemandeConge
    {
        public int Id { get; set; }

        public int? EmployeId { get; set; }
        public Employe? Employe { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateFin { get; set; }

        public string Statut { get; set; } = "En attente"; // En attente, Approuvée, Refusée

        [NotMapped]
        public int Duree => (DateFin - DateDebut).Days + 1;
    }

}
