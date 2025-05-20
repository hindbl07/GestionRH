namespace GestionRH.Models.DTOModels
{
    public class PosteDto
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal SalaireBase { get; set; }

        public int? DepartementId { get; set; }
        public string? DepartementNom { get; set; }
    }

}
