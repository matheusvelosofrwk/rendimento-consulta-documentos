using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serasa
{
    public class SerasaScoreRequestMock
    {
        [JsonPropertyName("documento")]
        public string Documento { get; set; }

        [JsonPropertyName("tipoDocumento")]
        public string TipoDocumento { get; set; }

        [JsonPropertyName("usuario")]
        public string Usuario { get; set; }

        [JsonPropertyName("senha")]
        public string Senha { get; set; }
    }
}
