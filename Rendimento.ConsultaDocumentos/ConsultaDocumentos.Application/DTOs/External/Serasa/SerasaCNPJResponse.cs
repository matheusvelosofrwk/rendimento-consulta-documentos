namespace ConsultaDocumentos.Application.DTOs.External.Serasa
{
    /// <summary>
    /// Response da consulta de CNPJ no Serasa (SOAP)
    /// Operação: retornaDadosPJ
    /// </summary>
    public class SerasaCNPJResponse
    {
        public string? CNPJ { get; set; }
        public string? Nome { get; set; }
        public string? DataAbertura { get; set; }
        public string? Situacao { get; set; }
        public string? DataSituacao { get; set; }
        public string? Data { get; set; }
        public string? Hora { get; set; }
        public string? Erro { get; set; }

        public bool TemErro => !string.IsNullOrWhiteSpace(Erro);
        public bool Sucesso => !TemErro && !string.IsNullOrWhiteSpace(Nome);
    }
}
