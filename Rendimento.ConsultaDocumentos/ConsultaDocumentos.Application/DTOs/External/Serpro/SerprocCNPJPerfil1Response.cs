using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    public class SerprocCNPJPerfil1Response
    {
        [JsonPropertyName("cnpj")]
        public string? Cnpj { get; set; }

        [JsonPropertyName("nomeEmpresarial")]
        public string? NomeEmpresarial { get; set; }

        [JsonPropertyName("nomeFantasia")]
        public string? NomeFantasia { get; set; }

        [JsonPropertyName("situacaoCadastral")]
        public string? SituacaoCadastral { get; set; }

        [JsonPropertyName("estabelecimento")]
        public string? Estabelecimento { get; set; }

        [JsonPropertyName("codigoPais")]
        public string? CodigoPais { get; set; }

        [JsonPropertyName("nomePais")]
        public string? NomePais { get; set; }

        [JsonPropertyName("cidadeExterior")]
        public string? CidadeExterior { get; set; }

        [JsonPropertyName("erro")]
        public string? Erro { get; set; }
    }
}
