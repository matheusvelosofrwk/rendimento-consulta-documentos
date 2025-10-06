using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Results;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<IEnumerable<UserDTO>>> GetAllAsync();
        Task<Result<UserDTO>> GetByIdAsync(string id);
        Task<Result<UserDTO>> CreateAsync(CreateUserDTO createUserDto);
        Task<Result<UserDTO>> UpdateAsync(UpdateUserDTO updateUserDto);
        Task<Result<bool>> DeleteAsync(string id);
        Task<Result<bool>> ManageRolesAsync(ManageUserRolesDTO manageUserRolesDto);
    }
}
