namespace GestionRH.Models.DTOModels
{
    public class EmployeDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Matricule { get; set; } = string.Empty;
        public string? EmailPro { get; set; }
        public DateTime DateEmbauche { get; set; }
        public int SoldeConge { get; set; }
        public string Statut { get; set; } = string.Empty;

        public int? DepartementId { get; set; }
        public string? DepartementNom { get; set; }

        public int? PosteId { get; set; }
        public string? PosteTitre { get; set; }
    }
}


