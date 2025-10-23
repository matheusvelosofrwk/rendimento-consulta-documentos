using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Enums;

namespace ConsultaDocumentos.Domain.Intefaces
{
    public interface ILogAuditoriaRepository
    {
        // Métodos de leitura
        Task<LogAuditoria?> GetByIdAsync(Guid id);
        Task<IList<LogAuditoria>> GetAllAsync();
        Task<IList<LogAuditoria>> GetByAplicacaoAsync(Guid aplicacaoId);
        Task<IList<LogAuditoria>> GetByDocumentoNumeroAsync(string documentoNumero);
        Task<IList<LogAuditoria>> GetByDataAsync(DateTime dataInicio, DateTime dataFim);
        Task<IList<LogAuditoria>> GetByConsultaSucessoAsync(bool consultaSucesso);
        Task<IList<LogAuditoria>> GetWithFiltersAsync(
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string? numeroDocumento = null,
            Guid? aplicacaoProvedorId = null,
            TipoDocumento? tipoDocumento = null);

        // Método de inserção
        Task AddAsync(LogAuditoria logAuditoria);
    }
}
