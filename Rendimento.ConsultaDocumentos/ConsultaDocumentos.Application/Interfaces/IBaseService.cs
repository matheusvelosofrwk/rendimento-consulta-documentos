using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Domain.Base;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface IBaseService<TDTO, TEntity>
        where TDTO : BaseDTO
        where TEntity : BaseEntity
    {
        Task AddAsync(TDTO dto);

        Task DeleteAsync(Guid id);

        Task<IList<TDTO>> GetAllAsync();

        Task<TDTO?> GetByIdAsync(Guid id);

        Task UpdateAsync(TDTO dto);

    }
}
