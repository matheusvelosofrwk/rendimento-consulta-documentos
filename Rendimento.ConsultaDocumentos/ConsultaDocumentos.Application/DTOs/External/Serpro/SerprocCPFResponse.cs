using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    public class SerprocCPFResponse
    {
        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("dataNascimento")]
        public string? DataNascimento { get; set; }

        [JsonPropertyName("nomeMae")]
        public string? NomeMae { get; set; }

        [JsonPropertyName("sexo")]
        public string? Sexo { get; set; }

        [JsonPropertyName("tituloEleitor")]
        public string? TituloEleitor { get; set; }

        [JsonPropertyName("residenteExterior")]
        public string? ResidenteExterior { get; set; }

        [JsonPropertyName("anoObito")]
        public string? AnoObito { get; set; }

        [JsonPropertyName("situacaoCadastral")]
        public string? SituacaoCadastral { get; set; }

        [JsonPropertyName("codigoControle")]
        public string? CodigoControle { get; set; }

        [JsonPropertyName("erro")]
        public string? Erro { get; set; }
    }
}
