using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Web.Models
{
    public class AplicacaoComProvedoresViewModel
    {
        // Dados da Aplicação
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O nome da aplicação é obrigatório")]
        public string Nome { get; set; }

        public string Descricao { get; set; }

        public string Status { get; set; }

        [Display(Name = "Acesso SERPRO")]
        public bool Serpro { get; set; }

        // Configurações do Certificado Digital
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

        // Lista de Provedores Vinculados
        public List<AplicacaoProvedorViewModel> ProvedoresVinculados { get; set; } = new List<AplicacaoProvedorViewModel>();

        // Lista de Todos os Provedores Disponíveis (para seleção)
        public List<ProvedorViewModel> ProvedoresDisponiveis { get; set; } = new List<ProvedorViewModel>();
    }
}
