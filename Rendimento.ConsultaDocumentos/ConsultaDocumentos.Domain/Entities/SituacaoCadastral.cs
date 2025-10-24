using ConsultaDocumentos.Domain.Base;

namespace ConsultaDocumentos.Domain.Entities
{
    public class SituacaoCadastral : BaseEntity
    {
        public string Descricao { get; set; }

        public bool Ativo { get; set; }

        public char TipoPessoa { get; set; } // 'F' = Pessoa Física, 'J' = Pessoa Jurídica, 'A' = Ambos

        public DateTime DataCriacao { get; set; }

        // Método de validação
        public bool IsValidaParaTipoPessoa(char tipoPessoa)
        {
            // Situação com TipoPessoa 'A' (Ambos) é válida para qualquer tipo
            if (TipoPessoa == 'A')
                return true;

            // Caso contrário, TipoPessoa deve corresponder ao tipo do documento
            return TipoPessoa == tipoPessoa;
        }

        // Métodos auxiliares
        public bool IsPessoaFisica() => TipoPessoa == 'F';
        public bool IsPessoaJuridica() => TipoPessoa == 'J';
        public bool IsAmbos() => TipoPessoa == 'A';

        public string GetTipoPessoaDescricao()
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
