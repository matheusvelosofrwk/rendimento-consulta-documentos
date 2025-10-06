using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Web.Models
{
    public class ConsultaDocumentoViewModel
    {
        [Required(ErrorMessage = "O número do documento é obrigatório")]
        [Display(Name = "Número do Documento")]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de documento é obrigatório")]
        [Display(Name = "Tipo de Documento")]
        public string TipoDocumento { get; set; } = "CPF";

        [Display(Name = "Perfil CNPJ")]
        [Range(1, 3, ErrorMessage = "O perfil deve ser 1, 2 ou 3")]
        public int PerfilCNPJ { get; set; } = 3;

        [Required(ErrorMessage = "A aplicação é obrigatória")]
        [Display(Name = "Aplicação")]
        public Guid AplicacaoId { get; set; }
    }
}
