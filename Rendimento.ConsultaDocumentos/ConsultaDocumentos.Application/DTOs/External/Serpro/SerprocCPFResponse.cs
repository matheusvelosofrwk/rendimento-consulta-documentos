using System.Text.Json.Serialization;

namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    /// <summary>
    /// Response da consulta de CPF no Serpro (REST/JSON)
    /// </summary>
    public class SerprocCPFResponse
    {
        [JsonPropertyName("codRetorno")]
        public string CodRetorno { get; set; } = string.Empty;

        [JsonPropertyName("msgRetorno")]
        public string MsgRetorno { get; set; } = string.Empty;

        [JsonPropertyName("qtdeRegistrosRetornados")]
        public string QtdeRegistrosRetornados { get; set; } = string.Empty;

        [JsonPropertyName("retornoConsultada")]
        public List<SerprocCPFDados>? RetornoConsultada { get; set; }

        public bool Sucesso => CodRetorno == "0" && RetornoConsultada?.Any() == true;
        public bool TemErro => CodRetorno != "0";
    }

    public class SerprocCPFDados
    {
        [JsonPropertyName("codRetorno")]
        public string CodRetorno { get; set; } = string.Empty;

        [JsonPropertyName("msgRetorno")]
        public string MsgRetorno { get; set; } = string.Empty;

        [JsonPropertyName("msgInformativa")]
        public string? MsgInformativa { get; set; }

        [JsonPropertyName("cpfContribuinte")]
        public string? CpfContribuinte { get; set; }

        [JsonPropertyName("codSitCad")]
        public string? CodSitCad { get; set; }

        [JsonPropertyName("nomeContribuinte")]
        public string? NomeContribuinte { get; set; }

        [JsonPropertyName("nomeSocial")]
        public string? NomeSocial { get; set; }

        [JsonPropertyName("nomeMae")]
        public string? NomeMae { get; set; }

        [JsonPropertyName("codSexo")]
        public string? CodSexo { get; set; }

        [JsonPropertyName("dataNascimento")]
        public string? DataNascimento { get; set; }

        [JsonPropertyName("dataInscricao")]
        public string? DataInscricao { get; set; }

        [JsonPropertyName("anoObito")]
        public string? AnoObito { get; set; }

        [JsonPropertyName("codMunicNaturIBGE")]
        public string? CodMunicNaturIBGE { get; set; }

        [JsonPropertyName("indResidExt")]
        public string? IndResidExt { get; set; }

        [JsonPropertyName("codPaisResidExt")]
        public string? CodPaisResidExt { get; set; }

        [JsonPropertyName("nomePaisResidExt")]
        public string? NomePaisResidExt { get; set; }

        [JsonPropertyName("codOcupacaoPrinc")]
        public string? CodOcupacaoPrinc { get; set; }

        [JsonPropertyName("codNaturezaOcup")]
        public string? CodNaturezaOcup { get; set; }

        [JsonPropertyName("anoExercicioOcup")]
        public string? AnoExercicioOcup { get; set; }

        [JsonPropertyName("dataUltimaAtual")]
        public string? DataUltimaAtual { get; set; }

        [JsonPropertyName("listaMotivoAltSitCad")]
        public List<SerprocMotivoAlteracao>? ListaMotivoAltSitCad { get; set; }

        public bool DadosValidos => CodRetorno == "0000" && !string.IsNullOrEmpty(NomeContribuinte);
    }

    public class SerprocMotivoAlteracao
    {
        [JsonPropertyName("codMotivoAltSitCad")]
        public string? CodMotivoAltSitCad { get; set; }

        [JsonPropertyName("descMotivoAltSitCad")]
        public string? DescMotivoAltSitCad { get; set; }
    }
}
