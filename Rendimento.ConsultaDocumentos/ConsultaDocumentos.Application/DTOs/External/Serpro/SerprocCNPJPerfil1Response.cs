namespace ConsultaDocumentos.Application.DTOs.External.Serpro
{
    /// <summary>
    /// Response do Perfil 1 - Dados básicos
    /// </summary>
    public class SerprocCNPJPerfil1Response
    {
        public string? CNPJ { get; set; }
        public string? Estabelecimento { get; set; }
        public string? NomeEmpresarial { get; set; }
        public string? NomeFantasia { get; set; }
        public string? SituacaoCadastral { get; set; }
        public string? MotivoSituacao { get; set; }
        public string? DataSituacaoCadastral { get; set; }
        public string? Erro { get; set; }

        public bool TemErro => !string.IsNullOrWhiteSpace(Erro);
        public bool Sucesso => !TemErro && !string.IsNullOrWhiteSpace(NomeEmpresarial);
    }
}
