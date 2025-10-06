using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;
using ConsultaDocumentos.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ConsultaDocumentos.Infra.Data.Repositories
{
    public class ProvedorRepository : BaseRepository<Provedor>, IProvedorRepository
    {
        private readonly ApplicationDbContext _context;

        public ProvedorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Provedor?> GetByNomeAsync(string nome)
        {
            return await _context.Provedor
                .FirstOrDefaultAsync(p => p.Nome.ToUpper() == nome.ToUpper());
        }
    }
}
