namespace ConsultaDocumentos.Application.DTOs.External.Serasa
{
    /// <summary>
    /// Request para consulta de CPF no Serasa (SOAP)
    /// Operação: retornaDadosPF
    /// </summary>
    public class SerasaCPFRequest
    {
        public string StrLogin { get; set; } = string.Empty;
        public string StrSenha { get; set; } = string.Empty;
        public string StrDominio { get; set; } = string.Empty;
        public string StrDocumento { get; set; } = string.Empty;
    }
}
