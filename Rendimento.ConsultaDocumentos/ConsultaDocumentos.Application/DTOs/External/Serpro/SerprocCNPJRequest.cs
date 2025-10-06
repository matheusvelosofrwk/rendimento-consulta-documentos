using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    public class SerprocCNPJRequest
    {
        [JsonPropertyName("listadecnpj")]
        public string ListaDeCnpj { get; set; }

        [JsonPropertyName("cpfUsuario")]
        public string CpfUsuario { get; set; }
    }
}
