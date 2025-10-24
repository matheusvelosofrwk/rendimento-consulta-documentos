namespace ConsultaDocumentos.Application.DTOs
{
    public class SituacaoCadastralDTO : BaseDTO
    {
        public string Descricao { get; set; }

        public bool Ativo { get; set; }

        public char TipoPessoa { get; set; } // 'F' = Pessoa Física, 'J' = Pessoa Jurídica, 'A' = Ambos

        public DateTime DataCriacao { get; set; }
    }
}
