using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;
using ConsultaDocumentos.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ConsultaDocumentos.Infra.Data.Repositories
{
    public class LogAuditoriaRepository : ILogAuditoriaRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<LogAuditoria> _dbSet;

        public LogAuditoriaRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<LogAuditoria>();
        }

        public async Task<LogAuditoria?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(x => x.Aplicacao)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IList<LogAuditoria>> GetAllAsync()
        {
            return await _dbSet
                .Include(x => x.Aplicacao)
                .OrderByDescending(x => x.DataHoraConsulta)
                .ToListAsync();
        }

        public async Task<IList<LogAuditoria>> GetByAplicacaoAsync(Guid aplicacaoId)
        {
            return await _dbSet
                .Include(x => x.Aplicacao)
                .Where(x => x.AplicacaoId == aplicacaoId)
                .OrderByDescending(x => x.DataHoraConsulta)
                .ToListAsync();
        }

        public async Task<IList<LogAuditoria>> GetByDocumentoNumeroAsync(string documentoNumero)
        {
            return await _dbSet
                .Include(x => x.Aplicacao)
                .Where(x => x.DocumentoNumero == documentoNumero)
                .OrderByDescending(x => x.DataHoraConsulta)
                .ToListAsync();
        }

        public async Task<IList<LogAuditoria>> GetByDataAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _dbSet
                .Include(x => x.Aplicacao)
                .Where(x => x.DataHoraConsulta >= dataInicio && x.DataHoraConsulta <= dataFim)
                .OrderByDescending(x => x.DataHoraConsulta)
                .ToListAsync();
        }

        public async Task<IList<LogAuditoria>> GetByConsultaSucessoAsync(bool consultaSucesso)
        {
            return await _dbSet
                .Include(x => x.Aplicacao)
                .Where(x => x.ConsultaSucesso == consultaSucesso)
                .OrderByDescending(x => x.DataHoraConsulta)
                .ToListAsync();
        }

        public async Task<IList<LogAuditoria>> GetWithFiltersAsync(
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string? numeroDocumento = null,
            Guid? aplicacaoProvedorId = null,
            Domain.Enums.TipoDocumento? tipoDocumento = null)
        {
            var query = _dbSet.Include(x => x.Aplicacao).AsQueryable();

            if (dataInicio.HasValue)
                query = query.Where(x => x.DataHoraConsulta >= dataInicio.Value);

            if (dataFim.HasValue)
            {
                // Adicionar 23:59:59 ao final do dia
                var dataFimComHora = dataFim.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.DataHoraConsulta <= dataFimComHora);
            }

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
                query = query.Where(x => x.DocumentoNumero.Contains(numeroDocumento));

            if (aplicacaoProvedorId.HasValue)
                query = query.Where(x => x.AplicacaoId == aplicacaoProvedorId.Value);

            if (tipoDocumento.HasValue)
                query = query.Where(x => x.TipoDocumento == tipoDocumento.Value);

            return await query
                .OrderByDescending(x => x.DataHoraConsulta)
                .ToListAsync();
        }

        public async Task AddAsync(LogAuditoria logAuditoria)
        {
            await _dbSet.AddAsync(logAuditoria);
            await _context.SaveChangesAsync();
        }
    }
}
