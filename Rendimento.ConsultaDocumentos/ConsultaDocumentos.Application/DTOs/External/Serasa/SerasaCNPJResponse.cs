using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serasa
{
    public class SerasaCNPJResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }

        [JsonPropertyName("mensagem")]
        public string Mensagem { get; set; }

        [JsonPropertyName("dataConsulta")]
        public string DataConsulta { get; set; }

        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; }

        [JsonPropertyName("dados")]
        public SerasaCNPJDadosDTO? Dados { get; set; }
    }
}
