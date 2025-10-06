using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    public class SerprocCNPJPerfil3ResponseMock : SerprocCNPJPerfil2ResponseMock
    {
        [JsonPropertyName("socios")]
        public List<SerprocSocioDTOMock>? Socios { get; set; }
    }
}
