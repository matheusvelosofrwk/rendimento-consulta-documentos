using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Application.Results;
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

        public async Task<Result<TDTO>> AddAsync(TDTO dto)
        {
            try
            {
                var entity = _mapper.Map<TEntity>(dto);
                await _repository.AddAsync(entity);

                var result = _mapper.Map<TDTO>(entity);
                return Result<TDTO>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                return Result<TDTO>.FailureResult(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult(ex.Message);
            }
        }

        public async Task<Result<IList<TDTO>>> GetAllAsync()
        {
            try
            {
                var entities = await _repository.GetAllAsync();
                var maps = _mapper.Map<IList<TDTO>>(entities);

                return Result<IList<TDTO>>.SuccessResult(maps);
            }
            catch (Exception ex)
            {
                return Result<IList<TDTO>>.FailureResult(ex.Message);
            }
        }

        public async Task<Result<TDTO>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);

                if (entity == null)
                {
                    return Result<TDTO>.FailureResult("Registro não encontrado");
                }

                var mapping = _mapper.Map<TDTO>(entity);
                return Result<TDTO>.SuccessResult(mapping);
            }
            catch (Exception ex)
            {
                return Result<TDTO>.FailureResult(ex.Message);
            }
        }

        public async Task<Result<TDTO>> UpdateAsync(TDTO dto)
        {
            try
            {
                var entity = _mapper.Map<TEntity>(dto);
                await _repository.UpdateAsync(entity);

                var result = _mapper.Map<TDTO>(entity);
                return Result<TDTO>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                return Result<TDTO>.FailureResult(ex.Message);
            }
        }
    }
}
