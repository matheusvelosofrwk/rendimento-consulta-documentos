using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serasa
{
    public class SerasaCPFDadosDTOMock
    {
        [JsonPropertyName("identificacao")]
        public SerasaCPFIdentificacaoDTOMock? Identificacao { get; set; }

        [JsonPropertyName("documentos")]
        public SerasaCPFDocumentosDTOMock? Documentos { get; set; }

        [JsonPropertyName("endereco")]
        public SerasaEnderecoDTOMock? Endereco { get; set; }

        [JsonPropertyName("contato")]
        public SerasaContatoDTOMock? Contato { get; set; }

        [JsonPropertyName("dadosComplementares")]
        public SerasaCPFDadosComplementaresDTOMock? DadosComplementares { get; set; }

        [JsonPropertyName("analiseRisco")]
        public SerasaAnaliseRiscoCPFDTOMock? AnaliseRisco { get; set; }
    }

    public class SerasaCPFIdentificacaoDTOMock
    {
        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("dataNascimento")]
        public string? DataNascimento { get; set; }

        [JsonPropertyName("nomeMae")]
        public string? NomeMae { get; set; }

        [JsonPropertyName("sexo")]
        public string? Sexo { get; set; }

        [JsonPropertyName("situacaoCadastral")]
        public string? SituacaoCadastral { get; set; }

        [JsonPropertyName("codigoSituacao")]
        public string? CodigoSituacao { get; set; }
    }

    public class SerasaCPFDocumentosDTOMock
    {
        [JsonPropertyName("cpf")]
        public string? Cpf { get; set; }

        [JsonPropertyName("rg")]
        public string? Rg { get; set; }

        [JsonPropertyName("orgaoExpedidor")]
        public string? OrgaoExpedidor { get; set; }

        [JsonPropertyName("ufRg")]
        public string? UfRg { get; set; }

        [JsonPropertyName("dataExpedicaoRg")]
        public string? DataExpedicaoRg { get; set; }

        [JsonPropertyName("tituloEleitor")]
        public string? TituloEleitor { get; set; }
    }

    public class SerasaCPFDadosComplementaresDTOMock
    {
        [JsonPropertyName("profissao")]
        public string? Profissao { get; set; }

        [JsonPropertyName("rendaMensal")]
        public decimal? RendaMensal { get; set; }

        [JsonPropertyName("escolaridade")]
        public string? Escolaridade { get; set; }

        [JsonPropertyName("estadoCivil")]
        public string? EstadoCivil { get; set; }
    }

    public class SerasaAnaliseRiscoCPFDTOMock
    {
        [JsonPropertyName("score")]
        public int? Score { get; set; }

        [JsonPropertyName("classificacao")]
        public string? Classificacao { get; set; }

        [JsonPropertyName("dataUltimaAtualizacao")]
        public string? DataUltimaAtualizacao { get; set; }
    }
}
