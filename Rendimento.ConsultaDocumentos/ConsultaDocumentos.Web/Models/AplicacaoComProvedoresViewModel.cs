using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Web.Models
{
    public class AplicacaoComProvedoresViewModel
    {
        // Dados da Aplicação
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O nome da aplicação é obrigatório")]
        public string Nome { get; set; }

        public string Descricao { get; set; }

        public string Status { get; set; }

        [Display(Name = "Acesso SERPRO")]
        public bool Serpro { get; set; }

        // Lista de Provedores Vinculados
        public List<AplicacaoProvedorViewModel> ProvedoresVinculados { get; set; } = new List<AplicacaoProvedorViewModel>();

        // Lista de Todos os Provedores Disponíveis (para seleção)
        public List<ProvedorViewModel> ProvedoresDisponiveis { get; set; } = new List<ProvedorViewModel>();
    }
}
