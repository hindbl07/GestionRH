namespace GestionRH.Models.DTOModels
{
    public class CandidatDetailDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CV { get; set; } = string.Empty;
        public string Statut { get; set; } = "En attente";

        // Liste simplifiée des entretiens
        public List<EntretienDto> Entretiens { get; set; } = new List<EntretienDto>();
    }

}
