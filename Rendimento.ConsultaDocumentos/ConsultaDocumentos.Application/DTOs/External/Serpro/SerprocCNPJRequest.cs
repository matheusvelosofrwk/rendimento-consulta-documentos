namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    /// <summary>
    /// Request para consulta de CNPJ no Serpro (SOAP/XML)
    /// Operação: ConsultarCNPJP7_SC (Perfil 7 com Sistema Convenente)
    /// </summary>
    public class SerprocCNPJRequest
    {
        public string CNPJ { get; set; } = string.Empty;
        public string CPFUsuario { get; set; } = "25008464825"; // Constante do sistema
        public string SistemaConvenente { get; set; } = string.Empty;
        public int Perfil { get; set; } = 7; // 1, 2, 3, 7
    }
}
