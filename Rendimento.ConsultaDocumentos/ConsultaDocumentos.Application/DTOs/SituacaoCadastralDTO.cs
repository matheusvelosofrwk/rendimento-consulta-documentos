namespace ConsultaDocumentos.Application.DTOs
{
    public class SituacaoCadastralDTO : BaseDTO
    {
        public string Descricao { get; set; }

        public bool Ativo { get; set; }

        public DateTime DataCriacao { get; set; }
    }
}
