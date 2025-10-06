using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serasa
{
    public class SerasaHealthCheckResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("services")]
        public SerasaServicesStatusDTO? Services { get; set; }
    }

    public class SerasaServicesStatusDTO
    {
        [JsonPropertyName("cpf")]
        public string? Cpf { get; set; }

        [JsonPropertyName("cnpj")]
        public string? Cnpj { get; set; }

        [JsonPropertyName("score")]
        public string? Score { get; set; }
    }
}
