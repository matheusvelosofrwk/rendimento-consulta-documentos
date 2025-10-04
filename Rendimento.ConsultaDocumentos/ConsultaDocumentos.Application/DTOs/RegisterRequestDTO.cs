using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Application.DTOs
{
    public class RegisterRequestDTO
    {
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
        [Compare("Password", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmPassword { get; set; }
    }
}
