namespace GestionRH.Models.ViewModels
{
    public class EmployeViewModel
    {
        public string NomComplet { get; set; }
    }

    public class PosteViewModel
    {
        public string Titre { get; set; }
        public List<EmployeViewModel> Employes { get; set; }
    }

    public class DepartementViewModel
    {
        public string Nom { get; set; }
        public List<PosteViewModel> Postes { get; set; }
    }

    public class HierarchieViewModel
    {
        public List<DepartementViewModel> Departements { get; set; }
    }

}
