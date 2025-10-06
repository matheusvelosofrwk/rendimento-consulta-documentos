using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    public class SerprocCNPJPerfil2ResponseMock : SerprocCNPJPerfil1ResponseMock
    {
        [JsonPropertyName("dataAbertura")]
        public string? DataAbertura { get; set; }

        [JsonPropertyName("dataSituacaoCadastral")]
        public string? DataSituacaoCadastral { get; set; }

        [JsonPropertyName("naturezaJuridica")]
        public string? NaturezaJuridica { get; set; }

        [JsonPropertyName("cnAEPrincipal")]
        public string? CnAEPrincipal { get; set; }

        [JsonPropertyName("cnAESecundario")]
        public List<string>? CnAESecundario { get; set; }

        [JsonPropertyName("tipoLogradouro")]
        public string? TipoLogradouro { get; set; }

        [JsonPropertyName("logradouro")]
        public string? Logradouro { get; set; }

        [JsonPropertyName("numeroLogradouro")]
        public string? NumeroLogradouro { get; set; }

        [JsonPropertyName("complemento")]
        public string? Complemento { get; set; }

        [JsonPropertyName("bairro")]
        public string? Bairro { get; set; }

        [JsonPropertyName("cep")]
        public string? Cep { get; set; }

        [JsonPropertyName("uf")]
        public string? Uf { get; set; }

        [JsonPropertyName("codigoMunicipio")]
        public string? CodigoMunicipio { get; set; }

        [JsonPropertyName("nomeMunicipio")]
        public string? NomeMunicipio { get; set; }

        [JsonPropertyName("ddd1")]
        public string? Ddd1 { get; set; }

        [JsonPropertyName("telefone1")]
        public string? Telefone1 { get; set; }

        [JsonPropertyName("ddd2")]
        public string? Ddd2 { get; set; }

        [JsonPropertyName("telefone2")]
        public string? Telefone2 { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }
}
