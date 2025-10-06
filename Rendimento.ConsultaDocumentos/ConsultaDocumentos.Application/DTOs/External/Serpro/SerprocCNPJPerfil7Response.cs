namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    /// <summary>
    /// Response do Perfil 7 - Completo (com quadro societário)
    /// Este é o perfil utilizado pelo sistema
    /// </summary>
    public class SerprocCNPJPerfil7Response : SerprocCNPJPerfil3Response
    {
        public string? CPFResponsavel { get; set; }
        public string? NomeResponsavel { get; set; }
        public string? CapitalSocial { get; set; }
        public List<SerprocSocioDTO>? Sociedade { get; set; }
        public string? Porte { get; set; }
        public string? OpcaoSimples { get; set; }
        public string? OpcaoSIMEI { get; set; }
        public string? SituacaoEspecial { get; set; }
        public string? DataSituacaoEspecial { get; set; }
        public string? CidadeExterior { get; set; }
        public string? CodigoPais { get; set; }
        public string? NomePais { get; set; }
    }
}
