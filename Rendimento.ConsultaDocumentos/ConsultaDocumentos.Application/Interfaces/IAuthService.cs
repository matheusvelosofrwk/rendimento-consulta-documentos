using ConsultaDocumentos.Application.DTOs;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO loginRequest);
        Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO registerRequest);
    }
}
