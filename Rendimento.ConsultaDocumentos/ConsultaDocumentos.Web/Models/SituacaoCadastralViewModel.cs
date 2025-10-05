namespace ConsultaDocumentos.Web.Models
{
    public class SituacaoCadastralViewModel : BaseViewModel
    {
        public string Descricao { get; set; }

        public bool Ativo { get; set; }

        public DateTime DataCriacao { get; set; }
    }
}
