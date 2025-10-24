using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;
using ConsultaDocumentos.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ConsultaDocumentos.Infra.Data.Repositories
{
    public class SituacaoCadastralRepository : BaseRepository<SituacaoCadastral>, ISituacaoCadastralRepository
    {
        private readonly ApplicationDbContext _context;

        public SituacaoCadastralRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SituacaoCadastral>> GetByTipoPessoaAsync(char tipoPessoa)
        {
            // Retorna situações que sejam do tipo específico OU tipo 'A' (Ambos)
            return await _context.Set<SituacaoCadastral>()
                .Where(s => s.TipoPessoa == tipoPessoa || s.TipoPessoa == 'A')
                .Where(s => s.Ativo)
                .OrderBy(s => s.Descricao)
                .ToListAsync();
        }
    }
}
