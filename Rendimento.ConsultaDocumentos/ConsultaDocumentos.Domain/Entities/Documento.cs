using ConsultaDocumentos.Domain.Base;

namespace ConsultaDocumentos.Domain.Entities
{
    public class Documento : BaseEntity
    {
        // Campos principais
        public char TipoPessoa { get; set; }
        public string Numero { get; set; }
        public string Nome { get; set; }
        public DateTime DataConsulta { get; set; }
        public DateTime DataConsultaValidade { get; set; }
        public byte[]? RowVersion { get; set; }

        // Campos PJ (Pessoa Jurídica)
        public DateTime? DataAbertura { get; set; }
        public string? Inscricao { get; set; }
        public int? NaturezaJuridica { get; set; }
        public string? DescricaoNaturezaJuridica { get; set; }
        public string? Segmento { get; set; }
        public int? RamoAtividade { get; set; }
        public string? DescricaoRamoAtividade { get; set; }
        public string? NomeFantasia { get; set; }
        public int? MatrizFilialQtde { get; set; }
        public bool? Matriz { get; set; }
        public string? Porte { get; set; } // ME/EPP/Médio/Grande

        // Campos PF (Pessoa Física)
        public DateTime? DataNascimento { get; set; }
        public string? NomeMae { get; set; }
        public string? Sexo { get; set; }
        public string? TituloEleitor { get; set; }
        public bool? ResidenteExterior { get; set; }
        public int? AnoObito { get; set; }
        public string? NomeSocial { get; set; }

        // Campos de Situação
        public DateTime? DataSituacao { get; set; }
        public Guid? IdSituacao { get; set; }
        public string? CodControle { get; set; }
        public DateTime? DataFundacao { get; set; }
        public string? OrigemBureau { get; set; }
        public Guid? IdNacionalidade { get; set; }

        // Navigation Properties
        public virtual Nacionalidade? Nacionalidade { get; set; }
        public virtual SituacaoCadastral? SituacaoCadastral { get; set; }
        public virtual ICollection<Endereco> Enderecos { get; set; } = new List<Endereco>();
        public virtual ICollection<Telefone> Telefones { get; set; } = new List<Telefone>();
        public virtual ICollection<Email> Emails { get; set; } = new List<Email>();
        public virtual ICollection<QuadroSocietario> QuadrosSocietarios { get; set; } = new List<QuadroSocietario>();

        // Factory Methods
        public static Documento CriarPessoaFisica(
            string numero,
            string nome,
            DateTime dataNascimento,
            int validadeDias = 90)
        {
            return new Documento
            {
                Id = Guid.NewGuid(),
                TipoPessoa = 'F',
                Numero = numero,
                Nome = nome,
                DataNascimento = dataNascimento,
                DataConsulta = DateTime.UtcNow,
                DataConsultaValidade = DateTime.UtcNow.AddDays(validadeDias)
            };
        }

        public static Documento CriarPessoaJuridica(
            string numero,
            string nome,
            DateTime? dataAbertura = null,
            int validadeDias = 90)
        {
            return new Documento
            {
                Id = Guid.NewGuid(),
                TipoPessoa = 'J',
                Numero = numero,
                Nome = nome,
                DataAbertura = dataAbertura,
                DataConsulta = DateTime.UtcNow,
                DataConsultaValidade = DateTime.UtcNow.AddDays(validadeDias)
            };
        }

        // Métodos de Domínio
        public bool IsValido()
        {
            return DataConsultaValidade > DateTime.UtcNow;
        }

        public bool IsPessoaFisica()
        {
            return TipoPessoa == 'F';
        }

        public bool IsPessoaJuridica()
        {
            return TipoPessoa == 'J';
        }

        public bool PrecisaAtualizacao()
        {
            return !IsValido();
        }

        public void AtualizarDataConsulta(int validadeDias = 90)
        {
            DataConsulta = DateTime.UtcNow;
            DataConsultaValidade = DateTime.UtcNow.AddDays(validadeDias);
        }

        public void AdicionarEndereco(Endereco endereco)
        {
            Enderecos.Add(endereco);
        }

        public void RemoverEndereco(Endereco endereco)
        {
            Enderecos.Remove(endereco);
        }

        public void AdicionarTelefone(Telefone telefone)
        {
            Telefones.Add(telefone);
        }

        public void RemoverTelefone(Telefone telefone)
        {
            Telefones.Remove(telefone);
        }

        public void AdicionarEmail(Email email)
        {
            Emails.Add(email);
        }

        public void RemoverEmail(Email email)
        {
            Emails.Remove(email);
        }

        public void AdicionarQuadroSocietario(QuadroSocietario quadroSocietario)
        {
            if (!IsPessoaJuridica())
                throw new InvalidOperationException("Quadro societário só pode ser adicionado a pessoa jurídica");

            QuadrosSocietarios.Add(quadroSocietario);
        }

        public void RemoverQuadroSocietario(QuadroSocietario quadroSocietario)
        {
            QuadrosSocietarios.Remove(quadroSocietario);
        }
    }
}
