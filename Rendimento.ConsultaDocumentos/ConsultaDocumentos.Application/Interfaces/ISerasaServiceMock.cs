using ConsultaDocumentos.Application.DTOs.External.Serasa;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface ISerasaServiceMock
    {
        Task<SerasaCPFResponseMock> ConsultarCPFAsync(string cpf, string tipoConsulta = "COMPLETA", CancellationToken cancellationToken = default);
        Task<SerasaCNPJResponseMock> ConsultarCNPJAsync(string cnpj, string tipoConsulta = "COMPLETA", CancellationToken cancellationToken = default);
        Task<SerasaScoreResponseMock> ConsultarScoreAsync(string documento, string tipoDocumento, CancellationToken cancellationToken = default);
        Task<SerasaHealthCheckResponseMock> HealthCheckAsync(CancellationToken cancellationToken = default);
    }
}
