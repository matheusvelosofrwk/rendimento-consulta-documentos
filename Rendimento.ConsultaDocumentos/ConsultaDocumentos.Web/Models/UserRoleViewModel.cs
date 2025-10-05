namespace ConsultaDocumentos.Web.Models
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public List<RoleSelectionViewModel> Roles { get; set; } = new();
    }

    public class RoleSelectionViewModel
    {
        public string RoleName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}
