using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Base;

namespace ConsultaDocumentos.Application.Services
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : BaseEntity
    {
        private readonly IBaseRepository<TEntity> _repository;

        public BaseService(IBaseRepository<TEntity> repository)
        {
            _repository = repository;
        }

        public Task AddAsync(TEntity entity)
        {
            return _repository.AddAsync(entity);
        }

        public Task DeleteAsync(Guid id)
        {
            return _repository.DeleteAsync(id);
        }

        public Task<IList<TEntity>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<TEntity?> GetByIdAsync(Guid id)
        {
            return _repository.GetByIdAsync(id);
        }

        public Task UpdateAsync(TEntity entity)
        {
            return _repository.UpdateAsync(entity);
        }
    }
}
