using ConsultaDocumentos.Web.Enums;

namespace ConsultaDocumentos.Web.Models
{
    public class AplicacaoProvedorViewModel : BaseViewModel
    {
        public Guid AplicacaoId { get; set; }
        public Guid ProvedorId { get; set; }
        public int Ordem { get; set; }
        public StatusEnum Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public string? CriadoPor { get; set; }
        public string? AtualizadoPor { get; set; }

        // Propriedades adicionais para exibição
        public string? NomeAplicacao { get; set; }
        public string? NomeProvedor { get; set; }
    }
}
