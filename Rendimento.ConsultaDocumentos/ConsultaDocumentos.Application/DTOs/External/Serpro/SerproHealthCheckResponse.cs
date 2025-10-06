namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    /// <summary>
    /// Response do health check do Serpro
    /// </summary>
    public class SerproHealthCheckResponse
    {
        public string Status { get; set; } = "UNKNOWN";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Mensagem { get; set; }
        public long? TempoRespostaMs { get; set; }

        public bool Disponivel => Status.ToUpperInvariant() == "OK";
    }
}
