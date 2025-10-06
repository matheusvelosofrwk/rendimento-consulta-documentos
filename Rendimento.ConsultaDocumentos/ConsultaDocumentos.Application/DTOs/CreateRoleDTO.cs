using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Application.DTOs
{
    public class CreateRoleDTO
    {
        [Required(ErrorMessage = "O nome do perfil é obrigatório")]
        public string Name { get; set; } = string.Empty;
    }
}
