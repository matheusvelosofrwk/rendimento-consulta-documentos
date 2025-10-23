using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;
using ConsultaDocumentos.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ConsultaDocumentos.Infra.Data.Repositories
{
    public class AplicacaoProvedorRepository : BaseRepository<AplicacaoProvedor>, IAplicacaoProvedorRepository
    {
        private readonly ApplicationDbContext _context;

        public AplicacaoProvedorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AplicacaoProvedor>> GetByAplicacaoIdAsync(Guid aplicacaoId)
        {
            return await _context.Set<AplicacaoProvedor>()
                .Include(ap => ap.Aplicacao)
                .Include(ap => ap.Provedor)
                .Where(ap => ap.AplicacaoId == aplicacaoId)
                .OrderBy(ap => ap.Ordem)
                .ToListAsync();
        }
    }
}
