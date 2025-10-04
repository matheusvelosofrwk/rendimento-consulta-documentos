using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Application.DTOs
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Password { get; set; }
    }
}
