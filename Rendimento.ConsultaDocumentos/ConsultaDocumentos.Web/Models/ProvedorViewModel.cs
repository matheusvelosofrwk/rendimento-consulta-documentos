using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Web.Models
{
    public class ProvedorViewModel : BaseViewModel
    {
        public string Credencial { get; set; }

        public string Descricao { get; set; }

        public string EndpointUrl { get; set; }

        public string Nome { get; set; }

        public int Prioridade { get; set; }

        public string Status { get; set; }

        public int Timeout { get; set; }

        // NOVOS CAMPOS
        [Display(Name = "Caminho Certificado (.pfx)")]
        public string? EndCertificado { get; set; }

        [Display(Name = "Usuário")]
        public string? Usuario { get; set; }

        [Display(Name = "Senha")]
        [DataType(DataType.Password)]
        public string? Senha { get; set; }

        [Display(Name = "Domínio")]
        public string? Dominio { get; set; }

        [Display(Name = "Quota Mínima")]
        public int? QtdAcessoMinimo { get; set; }

        [Display(Name = "Quota Máxima")]
        public int? QtdAcessoMaximo { get; set; }

        [Display(Name = "Validade CPF (dias)")]
        [Range(1, 365)]
        public int QtdDiasValidadePF { get; set; } = 30;

        [Display(Name = "Validade CNPJ (dias)")]
        [Range(1, 365)]
        public int QtdDiasValidadePJ { get; set; } = 30;

        [Display(Name = "Validade Endereço (dias)")]
        [Range(1, 365)]
        public int QtdDiasValidadeEND { get; set; } = 30;

        [Display(Name = "Min. Emails para Alerta")]
        public int? QtdMinEmailLog { get; set; }

        [Display(Name = "Dia de Corte")]
        [Range(1, 31)]
        public int? DiaCorte { get; set; }

        [Display(Name = "Porta (Socket)")]
        public int? Porta { get; set; }

        [Display(Name = "Tipo de Documento")]
        public int TipoWebService { get; set; } = 3; // 1=CPF, 2=CNPJ, 3=Ambos
    }
}
