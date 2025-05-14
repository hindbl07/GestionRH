using GestionRH.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GestionRH.Models.ViewModels
{
    public class EntretienViewModel
    {
        public Entretien Entretien { get; set; }

        public List<SelectListItem> Candidats { get; set; } = new();

        public List<SelectListItem> Utilisateurs { get; set; } = new();
    }
}
