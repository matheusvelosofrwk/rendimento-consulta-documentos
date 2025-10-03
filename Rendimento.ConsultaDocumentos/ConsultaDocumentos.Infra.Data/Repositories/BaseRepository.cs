using ConsultaDocumentos.Domain.Base;
using ConsultaDocumentos.Infra.Data.Context;

namespace ConsultaDocumentos.Infra.Data.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        public BaseRepository(ApplicationDbContext context)
        {
            
        }

        public Task<TEntity> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
