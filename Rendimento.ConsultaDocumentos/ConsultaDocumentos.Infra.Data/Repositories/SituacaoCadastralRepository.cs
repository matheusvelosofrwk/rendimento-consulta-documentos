using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;
using ConsultaDocumentos.Infra.Data.Context;

namespace ConsultaDocumentos.Infra.Data.Repositories
{
    public class SituacaoCadastralRepository : BaseRepository<SituacaoCadastral>, ISituacaoCadastralRepository
    {
        public SituacaoCadastralRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
