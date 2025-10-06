using ConsultaDocumentos.Application.DTOs.External.Serasa;

namespace ConsultaDocumentos.Application.Interfaces
{
    /// <summary>
    /// Interface para integração real com Serasa (SOAP)
    /// </summary>
    public interface ISerasaService
    {
        /// <summary>
        /// Consulta dados de CPF no Serasa
        /// </summary>
        /// <param name="cpf">CPF (apenas dígitos)</param>
        /// <param name="idSistema">ID do sistema solicitante</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Dados do CPF</returns>
        Task<SerasaCPFResponse> ConsultarCPFAsync(
            string cpf,
            int idSistema,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Consulta dados de CNPJ no Serasa
        /// </summary>
        /// <param name="cnpj">CNPJ (apenas dígitos)</param>
        /// <param name="idSistema">ID do sistema solicitante</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Dados do CNPJ</returns>
        Task<SerasaCNPJResponse> ConsultarCNPJAsync(
            string cnpj,
            int idSistema,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica disponibilidade do serviço Serasa
        /// </summary>
        Task<SerasaHealthCheckResponse> VerificarDisponibilidadeAsync(
            CancellationToken cancellationToken = default);
    }
}
