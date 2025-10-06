using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    public class SerprocSocioDTOMock
    {
        [JsonPropertyName("cpfCnpj")]
        public string? CpfCnpj { get; set; }

        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("qualificacao")]
        public string? Qualificacao { get; set; }

        [JsonPropertyName("dataEntrada")]
        public string? DataEntrada { get; set; }

        [JsonPropertyName("percentualCapital")]
        public string? PercentualCapital { get; set; }

        [JsonPropertyName("cpfRepresentanteLegal")]
        public string? CpfRepresentanteLegal { get; set; }

        [JsonPropertyName("nomeRepresentanteLegal")]
        public string? NomeRepresentanteLegal { get; set; }

        [JsonPropertyName("qualificacaoRepresentanteLegal")]
        public string? QualificacaoRepresentanteLegal { get; set; }
    }
}
