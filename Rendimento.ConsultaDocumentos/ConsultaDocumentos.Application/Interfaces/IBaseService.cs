using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Results;
using ConsultaDocumentos.Domain.Base;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface IBaseService<TDTO, TEntity>
        where TDTO : BaseDTO
        where TEntity : BaseEntity
    {
        Task<Result<TDTO>> AddAsync(TDTO dto);

        Task<Result<bool>> DeleteAsync(Guid id);

        Task<Result<IList<TDTO>>> GetAllAsync();

        Task<Result<TDTO>> GetByIdAsync(Guid id);

        Task<Result<TDTO>> UpdateAsync(TDTO dto);

    }
}
