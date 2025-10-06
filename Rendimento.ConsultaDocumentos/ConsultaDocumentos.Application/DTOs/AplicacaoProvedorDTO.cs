using ConsultaDocumentos.Domain.Enums;

namespace ConsultaDocumentos.Application.DTOs
{
    public class AplicacaoProvedorDTO : BaseDTO
    {
        public Guid AplicacaoId { get; set; }
        public Guid ProvedorId { get; set; }
        public int Ordem { get; set; }
        public StatusEnum Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public string? CriadoPor { get; set; }
        public string? AtualizadoPor { get; set; }

        // Campos de LOG DE USO (billing)
        public Guid? IdDocumento { get; set; }
        public DateTime? DataConsulta { get; set; }
        public string? EnderecoIP { get; set; }
        public string? RemoteHost { get; set; }

        // Propriedades adicionais para exibição
        public string? NomeAplicacao { get; set; }
        public string? NomeProvedor { get; set; }
    }
}
