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

        public async Task AddAsync(LogAuditoria logAuditoria)
        {
            await _dbSet.AddAsync(logAuditoria);
            await _context.SaveChangesAsync();
        }
    }
}
