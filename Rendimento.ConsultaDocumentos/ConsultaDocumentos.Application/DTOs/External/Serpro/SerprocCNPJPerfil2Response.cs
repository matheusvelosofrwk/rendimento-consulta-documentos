namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    /// <summary>
    /// Response do Perfil 2 - Dados básicos + Endereço
    /// </summary>
    public class SerprocCNPJPerfil2Response : SerprocCNPJPerfil1Response
    {
        public string? DataAbertura { get; set; }
        public string? NaturezaJuridica { get; set; }
        public string? TipoLogradouro { get; set; }
        public string? Logradouro { get; set; }
        public string? NumeroLogradouro { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? CEP { get; set; }
        public string? UF { get; set; }
        public string? CodigoMunicipio { get; set; }
        public string? NomeMunicipio { get; set; }
        public string? Referencia { get; set; }
    }
}
