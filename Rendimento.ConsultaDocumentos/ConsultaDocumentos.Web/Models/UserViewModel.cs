using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Web.Models
{
    public class UserViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Nome de Usuário")]
        public string? UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [Compare("Password", ErrorMessage = "As senhas não conferem")]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "Perfis")]
        public List<string>? Roles { get; set; }

        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
    }
}
