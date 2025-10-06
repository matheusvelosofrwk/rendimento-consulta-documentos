using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Web.Models
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "O nome do perfil é obrigatório")]
        [Display(Name = "Nome do Perfil")]
        [StringLength(256, ErrorMessage = "O nome deve ter no máximo 256 caracteres")]
        public string Name { get; set; } = string.Empty;
    }
}
