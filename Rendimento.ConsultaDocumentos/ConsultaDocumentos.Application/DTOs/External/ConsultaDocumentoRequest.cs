using ConsultaDocumentos.Domain.Enums;

namespace ConsultaDocumentos.Application.DTOs.External
{
    public class ConsultaDocumentoRequest
    {
        public string NumeroDocumento { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public int PerfilCNPJ { get; set; } = 1; // 1, 2 ou 3 (apenas para CNPJ)
        public Guid AplicacaoId { get; set; }
        public bool ForcarNovaConsulta { get; set; } = false;
    }
}
