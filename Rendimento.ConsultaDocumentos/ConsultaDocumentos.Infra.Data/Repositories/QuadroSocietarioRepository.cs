using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;
using ConsultaDocumentos.Infra.Data.Context;

namespace ConsultaDocumentos.Infra.Data.Repositories
{
    public class QuadroSocietarioRepository : BaseRepository<QuadroSocietario>, IQuadroSocietarioRepository
    {
        public QuadroSocietarioRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
