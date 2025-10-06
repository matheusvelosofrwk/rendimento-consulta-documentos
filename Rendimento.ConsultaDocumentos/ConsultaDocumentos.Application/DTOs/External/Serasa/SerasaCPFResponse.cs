namespace ConsultaDocumentos.Application.DTOs.External.Serasa
{
    /// <summary>
    /// Response da consulta de CPF no Serasa (SOAP)
    /// Operação: retornaDadosPF
    /// </summary>
    public class SerasaCPFResponse
    {
        public string? CPF { get; set; }
        public string? Nome { get; set; }
        public string? Situacao { get; set; }
        public string? Codigo_de_Controle { get; set; }
        public string? Hora { get; set; }
        public string? Fonte_Pesquisada { get; set; }
        public string? Erro { get; set; }

        public bool TemErro => !string.IsNullOrWhiteSpace(Erro);
        public bool Sucesso => !TemErro && !string.IsNullOrWhiteSpace(Nome);
    }
}
