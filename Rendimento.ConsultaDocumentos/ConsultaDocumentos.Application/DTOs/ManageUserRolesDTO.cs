using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Application.DTOs
{
    public class ManageUserRolesDTO
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public List<string> RoleNames { get; set; } = new List<string>();
    }
}
