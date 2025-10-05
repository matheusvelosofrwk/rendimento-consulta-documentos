using ConsultaDocumentos.Domain.Base;
using ConsultaDocumentos.Domain.Enums;

namespace ConsultaDocumentos.Domain.Entities
{
    public class LogAuditoria : BaseEntity
    {
        // Campos principais
        public Guid AplicacaoId { get; private set; }
        public string NomeAplicacao { get; private set; }
        public string DocumentoNumero { get; private set; }
        public TipoDocumento TipoDocumento { get; private set; }

        // Parâmetros e resultado
        public string? ParametrosEntrada { get; private set; }
        public string? ProvedoresUtilizados { get; private set; }
        public string? ProvedorPrincipal { get; private set; }
        public bool ConsultaSucesso { get; private set; }
        public string? RespostaProvedor { get; private set; }
        public string? MensagemRetorno { get; private set; }

        // Métricas
        public long TempoProcessamentoMs { get; private set; }
        public DateTime DataHoraConsulta { get; private set; }

        // Informações de requisição
        public string? EnderecoIp { get; private set; }
        public string? UserAgent { get; private set; }
        public string? TokenAutenticacao { get; private set; }

        // Controle
        public bool OrigemCache { get; private set; }
        public string? InformacoesAdicionais { get; private set; }

        // Navigation Properties
        public virtual Aplicacao? Aplicacao { get; private set; }

        // Construtor privado para EF Core
        private LogAuditoria() { }

        // Factory Method
        public static LogAuditoria Criar(
            Guid aplicacaoId,
            string nomeAplicacao,
            string documentoNumero,
            TipoDocumento tipoDocumento,
            bool consultaSucesso,
            long tempoProcessamentoMs,
            DateTime dataHoraConsulta,
            string? parametrosEntrada = null,
            string? provedoresUtilizados = null,
            string? provedorPrincipal = null,
            string? respostaProvedor = null,
            string? mensagemRetorno = null,
            string? enderecoIp = null,
            string? userAgent = null,
            string? tokenAutenticacao = null,
            bool origemCache = false,
            string? informacoesAdicionais = null)
        {
            return new LogAuditoria
            {
                Id = Guid.NewGuid(),
                AplicacaoId = aplicacaoId,
                NomeAplicacao = nomeAplicacao,
                DocumentoNumero = documentoNumero,
                TipoDocumento = tipoDocumento,
                ConsultaSucesso = consultaSucesso,
                TempoProcessamentoMs = tempoProcessamentoMs,
                DataHoraConsulta = dataHoraConsulta,
                ParametrosEntrada = parametrosEntrada,
                ProvedoresUtilizados = provedoresUtilizados,
                ProvedorPrincipal = provedorPrincipal,
                RespostaProvedor = respostaProvedor,
                MensagemRetorno = mensagemRetorno,
                EnderecoIp = enderecoIp,
                UserAgent = userAgent,
                TokenAutenticacao = tokenAutenticacao,
                OrigemCache = origemCache,
                InformacoesAdicionais = informacoesAdicionais
            };
        }

        // Métodos de domínio
        public bool IsConsultaComSucesso() => ConsultaSucesso;

        public bool IsOrigemCache() => OrigemCache;

        public bool IsSlow() => TempoProcessamentoMs > 5000; // > 5 segundos

        public string GetDescricaoTipoDocumento()
        {
            return TipoDocumento == TipoDocumento.CPF ? "CPF" : "CNPJ";
        }
    }
}
