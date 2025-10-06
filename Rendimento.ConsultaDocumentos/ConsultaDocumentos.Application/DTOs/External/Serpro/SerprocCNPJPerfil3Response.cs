using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    public class SerprocCNPJPerfil3Response : SerprocCNPJPerfil2Response
    {
        [JsonPropertyName("socios")]
        public List<SerprocSocioDTO>? Socios { get; set; }
    }
}
