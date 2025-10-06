namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    /// <summary>
    /// Response do Perfil 3 - Dados básicos + Endereço + Atividade
    /// </summary>
    public class SerprocCNPJPerfil3Response : SerprocCNPJPerfil2Response
    {
        public string? CNAEPrincipal { get; set; }
        public List<string>? CNAESecundario { get; set; }
        public string? DDD1 { get; set; }
        public string? Telefone1 { get; set; }
        public string? DDD2 { get; set; }
        public string? Telefone2 { get; set; }
        public string? Email { get; set; }
    }
}
