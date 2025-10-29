namespace ConsultaDocumentos.Application.DTOs
{
    public class AplicacaoDTO : BaseDTO
    {
        public string Nome { get; set; }

        public string Descricao { get; set; }

        public string Status { get; set; }

        public bool Serpro { get; set; }

        // Configurações do Certificado Digital
        public string? CertificadoCaminho { get; set; }

        public string? CertificadoSenha { get; set; }

        public string? CertificadoSenhaCriptografada { get; set; }

        public DateTime? CertificadoDataExpiracao { get; set; }
    }
}
