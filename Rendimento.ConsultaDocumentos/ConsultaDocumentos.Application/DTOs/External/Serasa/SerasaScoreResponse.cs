using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serasa
{
    public class SerasaScoreResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }

        [JsonPropertyName("mensagem")]
        public string Mensagem { get; set; }

        [JsonPropertyName("dataConsulta")]
        public string DataConsulta { get; set; }

        [JsonPropertyName("documento")]
        public string Documento { get; set; }

        [JsonPropertyName("tipoDocumento")]
        public string TipoDocumento { get; set; }

        [JsonPropertyName("score")]
        public SerasaScoreDataDTO? Score { get; set; }
    }

    public class SerasaScoreDataDTO
    {
        [JsonPropertyName("valor")]
        public int? Valor { get; set; }

        [JsonPropertyName("classificacao")]
        public string? Classificacao { get; set; }

        [JsonPropertyName("faixa")]
        public string? Faixa { get; set; }

        [JsonPropertyName("dataCalculo")]
        public string? DataCalculo { get; set; }

        [JsonPropertyName("fatoresPositivos")]
        public List<string>? FatoresPositivos { get; set; }

        [JsonPropertyName("fatoresNegativos")]
        public List<string>? FatoresNegativos { get; set; }
    }
}
