using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Web.Models
{
    public class ManageUserRolesViewModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public List<string> RoleNames { get; set; } = new List<string>();
    }
}
