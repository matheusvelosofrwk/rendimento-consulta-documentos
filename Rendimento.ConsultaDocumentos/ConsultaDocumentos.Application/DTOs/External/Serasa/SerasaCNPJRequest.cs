namespace ConsultaDocumentos.Application.DTOs.External.Serasa
{
    /// <summary>
    /// Request para consulta de CNPJ no Serasa (SOAP)
    /// Operação: retornaDadosPJ
    /// </summary>
    public class SerasaCNPJRequest
    {
        public string StrLogin { get; set; } = string.Empty;
        public string StrSenha { get; set; } = string.Empty;
        public string StrDominio { get; set; } = string.Empty;
        public string StrDocumento { get; set; } = string.Empty;
    }
}
