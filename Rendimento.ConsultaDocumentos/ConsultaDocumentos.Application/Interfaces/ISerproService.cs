using ConsultaDocumentos.Application.DTOs.External.Serpro;

namespace ConsultaDocumentos.Application.Interfaces
{
    /// <summary>
    /// Interface para integração real com Serpro (REST para CPF, SOAP para CNPJ)
    /// </summary>
    public interface ISerproService
    {
        /// <summary>
        /// Consulta dados de CPF no Serpro (REST/JSON)
        /// </summary>
        /// <param name="cpf">CPF (apenas dígitos)</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Dados do CPF</returns>
        Task<SerprocCPFResponse> ConsultarCPFAsync(
            string cpf,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Consulta dados de CNPJ no Serpro - Perfil 1 (SOAP/XML)
        /// </summary>
        Task<SerprocCNPJPerfil1Response> ConsultarCNPJPerfil1Async(
            string cnpj,
            string sistemaConvenente,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Consulta dados de CNPJ no Serpro - Perfil 2 (SOAP/XML)
        /// </summary>
        Task<SerprocCNPJPerfil2Response> ConsultarCNPJPerfil2Async(
            string cnpj,
            string sistemaConvenente,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Consulta dados de CNPJ no Serpro - Perfil 3 (SOAP/XML)
        /// </summary>
        Task<SerprocCNPJPerfil3Response> ConsultarCNPJPerfil3Async(
            string cnpj,
            string sistemaConvenente,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Consulta dados de CNPJ no Serpro - Perfil 7 completo com quadro societário (SOAP/XML)
        /// </summary>
        Task<SerprocCNPJPerfil7Response> ConsultarCNPJPerfil7Async(
            string cnpj,
            string sistemaConvenente,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica disponibilidade do serviço Serpro
        /// </summary>
        Task<SerproHealthCheckResponse> VerificarDisponibilidadeAsync(
            CancellationToken cancellationToken = default);
    }
}
