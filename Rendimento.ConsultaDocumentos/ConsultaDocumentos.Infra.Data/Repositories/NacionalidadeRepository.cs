using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;
using ConsultaDocumentos.Infra.Data.Context;

namespace ConsultaDocumentos.Infra.Data.Repositories
{
    public class NacionalidadeRepository : BaseRepository<Nacionalidade>, INacionalidadeRepository
    {
        public NacionalidadeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
