using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Application.DTOs
{
    public class UpdateUserDTO
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; }

        // Password é opcional no update - se vazio, não altera
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
        public string? Password { get; set; }
    }
}
