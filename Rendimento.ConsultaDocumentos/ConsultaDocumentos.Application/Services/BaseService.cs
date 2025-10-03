using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Base;

namespace ConsultaDocumentos.Application.Services
{
    public class BaseService<TDTO, TEntity> : IBaseService<TDTO, TEntity> 
        where TDTO : BaseDTO
        where TEntity : BaseEntity
    {
        protected readonly IBaseRepository<TEntity> _repository;
        protected readonly IMapper _mapper;

        public BaseService(IBaseRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public Task AddAsync(TDTO dto)
        {
            var entity = _mapper.Map<TEntity>(dto);

            return _repository.AddAsync(entity);
        }

        public Task DeleteAsync(Guid id)
        {
            return _repository.DeleteAsync(id);
        }

        public async Task<IList<TDTO>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            var maps = _mapper.Map<IList<TDTO>>(entities);

            return maps;
        }

        public async Task<TDTO?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            var mapping = _mapper.Map<TDTO>(entity);

            return mapping;
        }

        public Task UpdateAsync(TDTO dto)
        {
            var entity = _mapper.Map<TEntity>(dto);

            return _repository.UpdateAsync(entity);
        }
    }
}
