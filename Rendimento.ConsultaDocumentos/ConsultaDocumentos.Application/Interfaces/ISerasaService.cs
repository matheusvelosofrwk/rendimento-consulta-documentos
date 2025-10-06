using ConsultaDocumentos.Application.DTOs.External.Serasa;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface ISerasaService
    {
        Task<SerasaCPFResponse> ConsultarCPFAsync(string cpf, string tipoConsulta = "COMPLETA", CancellationToken cancellationToken = default);
        Task<SerasaCNPJResponse> ConsultarCNPJAsync(string cnpj, string tipoConsulta = "COMPLETA", CancellationToken cancellationToken = default);
        Task<SerasaScoreResponse> ConsultarScoreAsync(string documento, string tipoDocumento, CancellationToken cancellationToken = default);
        Task<SerasaHealthCheckResponse> HealthCheckAsync(CancellationToken cancellationToken = default);
    }
}
