using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serasa
{
    public class SerasaCPFResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }

        [JsonPropertyName("mensagem")]
        public string Mensagem { get; set; }

        [JsonPropertyName("dataConsulta")]
        public string DataConsulta { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("dados")]
        public SerasaCPFDadosDTO? Dados { get; set; }
    }
}
