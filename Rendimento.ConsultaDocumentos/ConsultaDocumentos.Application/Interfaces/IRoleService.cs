using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Results;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface IRoleService
    {
        Task<Result<IEnumerable<RoleDTO>>> GetAllAsync();
        Task<Result<RoleDTO>> GetByIdAsync(string id);
        Task<Result<RoleDTO>> CreateAsync(CreateRoleDTO createRoleDto);
        Task<Result<RoleDTO>> UpdateAsync(UpdateRoleDTO updateRoleDto);
        Task<Result<bool>> DeleteAsync(string id);
        Task<bool> RoleExistsAsync(string name);
    }
}
