using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Web.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
        [Compare("Password", ErrorMessage = "As senhas não coincidem")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        public string ConfirmPassword { get; set; }
    }
}
