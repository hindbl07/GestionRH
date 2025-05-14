using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GestionRH.Models.ViewModels
{
    public class EditRoleViewModel
    {
        public RoleUtilisateur Role { get; set; } // Remplacer string par RoleUtilisateur
        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
