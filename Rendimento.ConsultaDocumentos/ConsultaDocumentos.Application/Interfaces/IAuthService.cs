using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Results;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponseDTO>> LoginAsync(LoginRequestDTO loginRequest);
        Task<Result<AuthResponseDTO>> RegisterAsync(RegisterRequestDTO registerRequest);
    }
}
