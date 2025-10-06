using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serasa
{
    public class SerasaCPFRequest
    {
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("tipoConsulta")]
        public string TipoConsulta { get; set; }

        [JsonPropertyName("usuario")]
        public string Usuario { get; set; }

        [JsonPropertyName("senha")]
        public string Senha { get; set; }
    }
}
