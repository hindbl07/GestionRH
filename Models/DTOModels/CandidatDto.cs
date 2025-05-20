namespace GestionRH.Models.DTOModels
{
    public class CandidatDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Statut { get; set; } = "En attente";
        // Pas de CV ici pour alléger la réponse (ou créer un DTO spécifique pour CV si besoin)
    }

}
