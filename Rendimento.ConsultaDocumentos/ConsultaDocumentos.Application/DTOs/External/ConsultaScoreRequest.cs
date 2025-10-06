using ConsultaDocumentos.Domain.Enums;

namespace ConsultaDocumentos.Application.DTOs.External
{
    public class ConsultaScoreRequest
    {
        public string NumeroDocumento { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public Guid AplicacaoId { get; set; }
    }
}
