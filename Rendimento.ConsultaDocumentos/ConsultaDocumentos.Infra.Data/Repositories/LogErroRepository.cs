using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Enums;
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

        public async Task<IList<LogErro>> GetWithFiltersAsync(
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string? numeroDocumento = null,
            Guid? aplicacaoProvedorId = null,
            TipoDocumento? tipoDocumento = null)
        {
            var query = _dbSet
                .Include(x => x.Sistema)
                .AsQueryable();

            // Filtro por data
            if (dataInicio.HasValue)
            {
                query = query.Where(x => x.DataHora >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                // Adiciona 23:59:59 para incluir todo o dia
                var dataFimComHora = dataFim.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.DataHora <= dataFimComHora);
            }

            // Filtro por número de documento
            // Como LogErro não tem relação direta com Documento, vamos buscar no campo Erro ou StackTrace
            if (!string.IsNullOrWhiteSpace(numeroDocumento))
            {
                query = query.Where(x =>
                    (x.Erro != null && x.Erro.Contains(numeroDocumento)) ||
                    (x.StackTrace != null && x.StackTrace.Contains(numeroDocumento)) ||
                    (x.Metodo != null && x.Metodo.Contains(numeroDocumento)));
            }

            // Filtro por aplicação provedor
            // Como LogErro não tem relação direta com AplicacaoProvedor, vamos filtrar pelo IdSistema (Aplicacao)
            if (aplicacaoProvedorId.HasValue)
            {
                query = query.Where(x => x.IdSistema == aplicacaoProvedorId.Value);
            }

            // Filtro por tipo de documento
            // Vamos buscar por CPF ou CNPJ no conteúdo do erro
            if (tipoDocumento.HasValue)
            {
                var tipoDocumentoTexto = tipoDocumento.Value == TipoDocumento.CPF ? "CPF" : "CNPJ";
                query = query.Where(x =>
                    (x.Erro != null && x.Erro.Contains(tipoDocumentoTexto)) ||
                    (x.Metodo != null && x.Metodo.Contains(tipoDocumentoTexto)));
            }

            return await query
                .OrderByDescending(x => x.DataHora)
                .ToListAsync();
        }
    }
}
