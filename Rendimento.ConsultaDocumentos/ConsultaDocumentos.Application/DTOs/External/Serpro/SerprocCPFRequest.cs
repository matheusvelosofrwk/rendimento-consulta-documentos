using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    public class SerprocCPFRequest
    {
        [JsonPropertyName("listadecpf")]
        public string ListaDeCpf { get; set; }

        [JsonPropertyName("cpfUsuario")]
        public string CpfUsuario { get; set; }
    }
}
