using ConsultaDocumentos.Domain.Base;

namespace ConsultaDocumentos.Domain.Entities
{
    public class Aplicacao : BaseEntity
    {
        public string Nome { get; set; }

        public string Descricao { get; set; }

        public string Status { get; set; }

        // Flag se sistema tem acesso ao SERPRO
        public bool Serpro { get; set; }

        // Configurações do Certificado Digital (para acesso ao Serpro)
        public string? CertificadoCaminho { get; set; } // Caminho do arquivo .pfx do certificado

        public string? CertificadoSenha { get; set; } // Senha do certificado (IMPORTANTE: deve ser criptografada antes de salvar)

        public string? CertificadoSenhaCriptografada { get; set; } // Senha criptografada com AES

        public DateTime? CertificadoDataExpiracao { get; set; } // Data de expiração do certificado
    }
}
