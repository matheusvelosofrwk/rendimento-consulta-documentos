namespace ConsultaDocumentos.Application.DTOs
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public DateTime Expiration { get; set; }
    }
}
