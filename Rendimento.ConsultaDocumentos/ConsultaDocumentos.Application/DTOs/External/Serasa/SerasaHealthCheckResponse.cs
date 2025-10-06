namespace ConsultaDocumentos.Application.DTOs.External.Serasa
{
    /// <summary>
    /// Response do health check do Serasa
    /// </summary>
    public class SerasaHealthCheckResponse
    {
        public string Status { get; set; } = "UNKNOWN";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Mensagem { get; set; }
        public long? TempoRespostaMs { get; set; }

        public bool Disponivel => Status.ToUpperInvariant() == "OK";
    }
}
