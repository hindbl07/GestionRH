namespace GestionRH.Models.DTOModels
{
    public class EntretienDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Lieu { get; set; } = string.Empty;
        public string Resultat { get; set; } = string.Empty;

        public int CandidatId { get; set; }
        public string? CandidatNom { get; set; }

        public string? UtilisateurId { get; set; }
        public string? UtilisateurNom { get; set; }
    }

}
