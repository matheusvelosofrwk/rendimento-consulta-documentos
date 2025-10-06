using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serasa
{
    public class SerasaCNPJDadosDTO
    {
        [JsonPropertyName("identificacao")]
        public SerasaCNPJIdentificacaoDTO? Identificacao { get; set; }

        [JsonPropertyName("atividade")]
        public SerasaCNPJAtividadeDTO? Atividade { get; set; }

        [JsonPropertyName("endereco")]
        public SerasaEnderecoDTO? Endereco { get; set; }

        [JsonPropertyName("contato")]
        public SerasaContatoDTO? Contato { get; set; }

        [JsonPropertyName("dadosComplementares")]
        public SerasaCNPJDadosComplementaresDTO? DadosComplementares { get; set; }

        [JsonPropertyName("quadroSocietario")]
        public List<SerasaQuadroSocietarioDTO>? QuadroSocietario { get; set; }

        [JsonPropertyName("analiseRisco")]
        public SerasaAnaliseRiscoCNPJDTO? AnaliseRisco { get; set; }
    }

    public class SerasaCNPJIdentificacaoDTO
    {
        [JsonPropertyName("razaoSocial")]
        public string? RazaoSocial { get; set; }

        [JsonPropertyName("nomeFantasia")]
        public string? NomeFantasia { get; set; }

        [JsonPropertyName("situacaoCadastral")]
        public string? SituacaoCadastral { get; set; }

        [JsonPropertyName("codigoSituacao")]
        public string? CodigoSituacao { get; set; }

        [JsonPropertyName("dataAbertura")]
        public string? DataAbertura { get; set; }

        [JsonPropertyName("dataSituacaoCadastral")]
        public string? DataSituacaoCadastral { get; set; }
    }

    public class SerasaCNPJAtividadeDTO
    {
        [JsonPropertyName("naturezaJuridica")]
        public string? NaturezaJuridica { get; set; }

        [JsonPropertyName("naturezaJuridicaDescricao")]
        public string? NaturezaJuridicaDescricao { get; set; }

        [JsonPropertyName("cnAEPrincipal")]
        public string? CnAEPrincipal { get; set; }

        [JsonPropertyName("cnAEPrincipalDescricao")]
        public string? CnAEPrincipalDescricao { get; set; }
    }

    public class SerasaCNPJDadosComplementaresDTO
    {
        [JsonPropertyName("capitalSocial")]
        public decimal? CapitalSocial { get; set; }

        [JsonPropertyName("porte")]
        public string? Porte { get; set; }

        [JsonPropertyName("dataUltimaAtualizacao")]
        public string? DataUltimaAtualizacao { get; set; }
    }

    public class SerasaQuadroSocietarioDTO
    {
        [JsonPropertyName("cpfCnpj")]
        public string? CpfCnpj { get; set; }

        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("qualificacao")]
        public string? Qualificacao { get; set; }

        [JsonPropertyName("qualificacaoDescricao")]
        public string? QualificacaoDescricao { get; set; }

        [JsonPropertyName("dataEntrada")]
        public string? DataEntrada { get; set; }

        [JsonPropertyName("percentualCapital")]
        public string? PercentualCapital { get; set; }
    }

    public class SerasaAnaliseRiscoCNPJDTO
    {
        [JsonPropertyName("score")]
        public int? Score { get; set; }

        [JsonPropertyName("classificacao")]
        public string? Classificacao { get; set; }

        [JsonPropertyName("dataUltimaAtualizacao")]
        public string? DataUltimaAtualizacao { get; set; }

        [JsonPropertyName("indicadores")]
        public SerasaIndicadoresRiscoDTO? Indicadores { get; set; }
    }

    public class SerasaIndicadoresRiscoDTO
    {
        [JsonPropertyName("protestos")]
        public int? Protestos { get; set; }

        [JsonPropertyName("acoesCiveis")]
        public int? AcoesCiveis { get; set; }

        [JsonPropertyName("falencias")]
        public int? Falencias { get; set; }

        [JsonPropertyName("concordatas")]
        public int? Concordatas { get; set; }
    }
}
