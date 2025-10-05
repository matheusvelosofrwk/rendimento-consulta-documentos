using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;
using ConsultaDocumentos.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ConsultaDocumentos.Infra.Data.Repositories
{
    public class LogErroRepository : ILogErroRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<LogErro> _dbSet;

        public LogErroRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<LogErro>();
        }

        public async Task<LogErro?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(x => x.Sistema)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IList<LogErro>> GetAllAsync()
        {
            return await _dbSet
                .Include(x => x.Sistema)
                .OrderByDescending(x => x.DataHora)
                .ToListAsync();
        }

        public async Task<IList<LogErro>> GetByAplicacaoAsync(string aplicacao)
        {
            return await _dbSet
                .Include(x => x.Sistema)
                .Where(x => x.Aplicacao == aplicacao)
                .OrderByDescending(x => x.DataHora)
                .ToListAsync();
        }

        public async Task<IList<LogErro>> GetByDataAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _dbSet
                .Include(x => x.Sistema)
                .Where(x => x.DataHora >= dataInicio && x.DataHora <= dataFim)
                .OrderByDescending(x => x.DataHora)
                .ToListAsync();
        }

        public async Task<IList<LogErro>> GetByUsuarioAsync(string usuario)
        {
            return await _dbSet
                .Include(x => x.Sistema)
                .Where(x => x.Usuario == usuario)
                .OrderByDescending(x => x.DataHora)
                .ToListAsync();
        }

        public async Task<IList<LogErro>> GetBySistemaAsync(Guid idSistema)
        {
            return await _dbSet
                .Include(x => x.Sistema)
                .Where(x => x.IdSistema == idSistema)
                .OrderByDescending(x => x.DataHora)
                .ToListAsync();
        }

        public async Task AddAsync(LogErro logErro)
        {
            await _dbSet.AddAsync(logErro);
            await _context.SaveChangesAsync();
        }
    }
}
