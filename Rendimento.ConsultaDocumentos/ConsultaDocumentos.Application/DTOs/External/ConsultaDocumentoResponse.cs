using ConsultaDocumentos.Domain.Enums;

namespace ConsultaDocumentos.Application.DTOs.External
{
    public class ConsultaDocumentoResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public string? ProvedorUtilizado { get; set; }
        public List<string>? ProvedoresTentados { get; set; }
        public bool OrigemCache { get; set; }
        public long TempoProcessamentoMs { get; set; }
        public DateTime DataConsulta { get; set; }
        public DocumentoDTO? Documento { get; set; }
        public string? Erro { get; set; }
    }
}
