using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Web.Models
{
    public class AplicacaoViewModel : BaseViewModel
    {
        public string Nome { get; set; }

        public string Descricao { get; set; }

        public string Status { get; set; }

        [Display(Name = "Acesso SERPRO")]
        public bool Serpro { get; set; }

        [Display(Name = "Caminho do Certificado")]
        [MaxLength(500, ErrorMessage = "O caminho do certificado deve ter no máximo 500 caracteres")]
        public string? CertificadoCaminho { get; set; }

        [Display(Name = "Senha do Certificado")]
        [DataType(DataType.Password)]
        [MaxLength(100, ErrorMessage = "A senha deve ter no máximo 100 caracteres")]
        public string? CertificadoSenha { get; set; }

        [Display(Name = "Senha Criptografada")]
        public string? CertificadoSenhaCriptografada { get; set; }

        [Display(Name = "Data de Expiração do Certificado")]
        [DataType(DataType.Date)]
        public DateTime? CertificadoDataExpiracao { get; set; }
    }
}
