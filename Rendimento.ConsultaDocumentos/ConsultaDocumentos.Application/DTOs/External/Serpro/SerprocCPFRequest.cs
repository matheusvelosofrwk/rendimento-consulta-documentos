using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    /// <summary>
    /// Request para consulta de CPF no Serpro (REST/JSON)
    /// </summary>
    public class SerprocCPFRequest
    {
        [JsonPropertyName("listadecpf")]
        public string Listadecpf { get; set; } = string.Empty;

        [JsonPropertyName("cpfUsuario")]
        public string CpfUsuario { get; set; } = "25008464825"; // Constante do sistema
    }
}
