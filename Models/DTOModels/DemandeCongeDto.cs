namespace GestionRH.Models.DTOModels
{
    public class DemandeCongeDto
    {
        public int Id { get; set; }
        public int? EmployeId { get; set; }
        public string? EmployeNom { get; set; }  // Pour affichage simple

        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string Statut { get; set; } = "En attente";

        public int Duree => (DateFin - DateDebut).Days + 1;
    }

}
