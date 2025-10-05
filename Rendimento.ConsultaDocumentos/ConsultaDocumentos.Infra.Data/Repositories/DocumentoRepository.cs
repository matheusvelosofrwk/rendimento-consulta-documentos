using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;
using ConsultaDocumentos.Infra.Data.Context;

namespace ConsultaDocumentos.Infra.Data.Repositories
{
    public class DocumentoRepository : BaseRepository<Documento>, IDocumentoRepository
    {
        public DocumentoRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
