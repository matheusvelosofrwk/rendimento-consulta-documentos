using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Web.Models
{
    public class SituacaoCadastralViewModel : BaseViewModel
    {
        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "A descrição é obrigatória")]
        [MaxLength(100, ErrorMessage = "A descrição deve ter no máximo 100 caracteres")]
        public string Descricao { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; }

        [Display(Name = "Tipo de Pessoa")]
        [Required(ErrorMessage = "O tipo de pessoa é obrigatório")]
        public char TipoPessoa { get; set; } = 'A'; // Default: Ambos

        [Display(Name = "ID Serpro")]
        [MaxLength(10, ErrorMessage = "O ID Serpro deve ter no máximo 10 caracteres")]
        public string? IdSerpro { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime DataCriacao { get; set; }

        // Helper para exibir o tipo de pessoa na view
        public string TipoPessoaDescricao
        {
            get
            {
                return TipoPessoa switch
                {
                    'F' => "Pessoa Física",
                    'J' => "Pessoa Jurídica",
                    'A' => "Ambos",
                    _ => "Não especificado"
                };
            }
        }
    }
}
