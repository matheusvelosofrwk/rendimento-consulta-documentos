using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Web.Models
{
    public class QuadroSocietarioViewModel : BaseViewModel
    {
        public Guid IdDocumento { get; set; }

        [Display(Name = "CPF/CNPJ")]
        public string? CpfCnpj { get; set; }

        [Display(Name = "Nome")]
        public string? Nome { get; set; }

        [Display(Name = "Qualificação")]
        public string? Qualificacao { get; set; }

        [Display(Name = "CPF Representante Legal")]
        public string? CpfRepresentanteLegal { get; set; }

        [Display(Name = "Nome Representante Legal")]
        public string? NomeRepresentanteLegal { get; set; }

        [Display(Name = "Qualificação Representante Legal")]
        public string? QualificacaoRepresentanteLegal { get; set; }

        [Display(Name = "Data Entrada")]
        public DateTime? DataEntrada { get; set; }

        [Display(Name = "Data Saída")]
        public DateTime? DataSaida { get; set; }

        [Display(Name = "Percentual Capital (%)")]
        public decimal? PercentualCapital { get; set; }

        // NOVOS CAMPOS
        [Display(Name = "Tipo de Sócio")]
        public string? Tipo { get; set; }

        [Display(Name = "Nacionalidade")]
        public Guid? IdNacionalidade { get; set; }

        [Display(Name = "Código País")]
        public string? CodPaisOrigem { get; set; }

        [Display(Name = "País de Origem")]
        public string? NomePaisOrigem { get; set; }

        // Para dropdown
        public NacionalidadeViewModel? Nacionalidade { get; set; }
    }
}
