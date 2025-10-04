using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;
using ConsultaDocumentos.Infra.Data.Context;

namespace ConsultaDocumentos.Infra.Data.Repositories
{
    public class ProvedorRepository : BaseRepository<Provedor>, IProvedorRepository
    {
        public ProvedorRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
