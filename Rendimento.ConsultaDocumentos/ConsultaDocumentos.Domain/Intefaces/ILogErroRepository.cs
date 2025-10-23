using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Enums;

namespace ConsultaDocumentos.Domain.Intefaces
{
    public interface ILogErroRepository
    {
        // Métodos de leitura
        Task<LogErro?> GetByIdAsync(Guid id);
        Task<IList<LogErro>> GetAllAsync();
        Task<IList<LogErro>> GetByAplicacaoAsync(string aplicacao);
        Task<IList<LogErro>> GetByDataAsync(DateTime dataInicio, DateTime dataFim);
        Task<IList<LogErro>> GetByUsuarioAsync(string usuario);
        Task<IList<LogErro>> GetBySistemaAsync(Guid idSistema);
        Task<IList<LogErro>> GetWithFiltersAsync(
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string? numeroDocumento = null,
            Guid? aplicacaoProvedorId = null,
            TipoDocumento? tipoDocumento = null);

        // Método de inserção
        Task AddAsync(LogErro logErro);
    }
}
