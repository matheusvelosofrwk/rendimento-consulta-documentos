using ConsultaDocumentos.Domain.Enums;

namespace ConsultaDocumentos.Application.DTOs.External
{
    public class ConsultaDocumentoRequest
    {
        public string NumeroDocumento { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public int PerfilCNPJ { get; set; } = 1; // 1, 2 ou 3 (apenas para CNPJ)
        public Guid AplicacaoId { get; set; }

        // Novos campos para melhorar a consulta
        public TipoConsulta TipoConsulta { get; set; } = TipoConsulta.ConsultaCompleta;
        public OrigemConsulta OrigemConsulta { get; set; } = OrigemConsulta.RepositorioEHubs;
        public bool ConsultarVencidos { get; set; } = false; // Se true, aceita documentos vencidos do cache
    }
}
