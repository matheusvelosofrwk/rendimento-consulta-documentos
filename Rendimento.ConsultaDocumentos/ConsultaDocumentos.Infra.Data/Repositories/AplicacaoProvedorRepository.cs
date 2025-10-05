using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;
using ConsultaDocumentos.Infra.Data.Context;

namespace ConsultaDocumentos.Infra.Data.Repositories
{
    public class AplicacaoProvedorRepository : BaseRepository<AplicacaoProvedor>, IAplicacaoProvedorRepository
    {
        public AplicacaoProvedorRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
