using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;
using ConsultaDocumentos.Infra.Data.Context;

namespace ConsultaDocumentos.Infra.Data.Repositories
{
    public class AplicacaoRepository : BaseRepository<Aplicacao>, IAplicacaoRepository
    {
        public AplicacaoRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
