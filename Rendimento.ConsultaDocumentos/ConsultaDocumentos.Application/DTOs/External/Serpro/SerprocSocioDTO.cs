namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    /// <summary>
    /// DTO de sócio do Serpro CNPJ
    /// </summary>
    public class SerprocSocioDTO
    {
        public string? CPFCNPJSocio { get; set; }
        public string? NomeSocio { get; set; }
        public string? QualificacaoSocio { get; set; }
        public string? DataEntrada { get; set; }
        public string? CPFRepresentante { get; set; }
        public string? NomeRepresentante { get; set; }
        public string? QualificacaoRepresentante { get; set; }
    }
}
