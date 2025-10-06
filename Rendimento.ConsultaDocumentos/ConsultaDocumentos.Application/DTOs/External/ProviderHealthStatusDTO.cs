namespace ConsultaDocumentos.Application.DTOs.External
{
    public class ProviderHealthStatusDTO
    {
        public string NomeProvedor { get; set; }
        public bool Disponivel { get; set; }
        public string Status { get; set; }
        public string? Mensagem { get; set; }
        public DateTime DataVerificacao { get; set; }
        public long TempoRespostaMs { get; set; }
    }

    public class ProvidersHealthCheckResponse
    {
        public bool AlgumProvedorDisponivel { get; set; }
        public int TotalProvedores { get; set; }
        public int ProvedoresDisponiveis { get; set; }
        public List<ProviderHealthStatusDTO> Provedores { get; set; } = new();
        public DateTime DataVerificacao { get; set; }
    }
}
