using System.Security.Cryptography.X509Certificates;

namespace ConsultaDocumentos.Domain.Interfaces
{
    public interface ICertificadoManager
    {
        X509Certificate2 ObterCertificado();
        Task<X509Certificate2> ObterCertificadoAsync(CancellationToken cancellationToken = default);
        bool CertificadoCarregado { get; }
        DateTime? DataExpiracao { get; }
    }
}
