using ConsultaDocumentos.Domain.Base;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface IBaseService<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity?> GetByIdAsync(Guid id);

        Task<IList<TEntity>> GetAllAsync();

        Task AddAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(Guid id);
    }
}
