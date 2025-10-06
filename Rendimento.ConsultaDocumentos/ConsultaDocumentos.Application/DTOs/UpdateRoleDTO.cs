using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Application.DTOs
{
    public class UpdateRoleDTO
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome do perfil é obrigatório")]
        public string Name { get; set; } = string.Empty;
    }
}
