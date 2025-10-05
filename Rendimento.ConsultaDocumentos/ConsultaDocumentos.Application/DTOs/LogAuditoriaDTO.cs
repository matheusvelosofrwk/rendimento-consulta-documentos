using ConsultaDocumentos.Domain.Enums;

namespace ConsultaDocumentos.Application.DTOs
{
    public class LogAuditoriaDTO
    {
        public Guid Id { get; set; }
        public Guid AplicacaoId { get; set; }
        public string NomeAplicacao { get; set; }
        public string DocumentoNumero { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public string? ParametrosEntrada { get; set; }
        public string? ProvedoresUtilizados { get; set; }
        public string? ProvedorPrincipal { get; set; }
        public bool ConsultaSucesso { get; set; }
        public string? RespostaProvedor { get; set; }
        public string? MensagemRetorno { get; set; }
        public long TempoProcessamentoMs { get; set; }
        public DateTime DataHoraConsulta { get; set; }
        public string? EnderecoIp { get; set; }
        public string? UserAgent { get; set; }
        public string? TokenAutenticacao { get; set; }
        public bool OrigemCache { get; set; }
        public string? InformacoesAdicionais { get; set; }
    }
}
