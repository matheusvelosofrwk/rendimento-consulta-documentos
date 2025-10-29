namespace ConsultaDocumentos.Application.DTOs
{
    public class SituacaoCadastralDTO : BaseDTO
    {
        public string Descricao { get; set; }

        public bool Ativo { get; set; }

        public char TipoPessoa { get; set; } // 'F' = Pessoa Física, 'J' = Pessoa Jurídica, 'A' = Ambos

        public string? IdSerpro { get; set; } // Código de situação cadastral retornado pelo Serpro

        public DateTime DataCriacao { get; set; }
    }
}
