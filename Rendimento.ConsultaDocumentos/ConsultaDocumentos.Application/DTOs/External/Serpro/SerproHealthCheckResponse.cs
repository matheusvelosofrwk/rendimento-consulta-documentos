using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    public class SerproHealthCheckResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
    }
}
