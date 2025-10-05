using ConsultaDocumentos.Domain.Base;

namespace ConsultaDocumentos.Domain.Entities
{
    public class QuadroSocietario : BaseEntity
    {
        public Guid IdDocumento { get; set; }

        // Campos legados
        public string? CPFSocio { get; set; }
        public string? NomeSocio { get; set; }
        public string? QualificacaoSocio { get; set; }

        // Campos novos
        public string? CpfCnpj { get; set; }
        public string? Nome { get; set; }
        public string? Qualificacao { get; set; }

        // Representante legal
        public string? CpfRepresentanteLegal { get; set; }
        public string? NomeRepresentanteLegal { get; set; }
        public string? QualificacaoRepresentanteLegal { get; set; }

        // Outras informações
        public DateTime? DataEntrada { get; set; }
        public DateTime? DataSaida { get; set; }
        public decimal? PercentualCapital { get; set; }
        public DateTime DataCriacao { get; set; }

        // Navigation Property
        public virtual Documento? Documento { get; set; }

        // Factory Method
        public static QuadroSocietario Criar(
            Guid idDocumento,
            string? cpfCnpj,
            string? nome,
            string? qualificacao)
        {
            return new QuadroSocietario
            {
                Id = Guid.NewGuid(),
                IdDocumento = idDocumento,
                CpfCnpj = cpfCnpj,
                Nome = nome,
                Qualificacao = qualificacao,
                DataCriacao = DateTime.UtcNow
            };
        }

        // Métodos de Domínio
        public bool IsValido()
        {
            var nomeValido = !string.IsNullOrWhiteSpace(Nome) || !string.IsNullOrWhiteSpace(NomeSocio);
            var qualificacaoValida = !string.IsNullOrWhiteSpace(Qualificacao) || !string.IsNullOrWhiteSpace(QualificacaoSocio);

            return nomeValido && qualificacaoValida;
        }

        public string GetPercentualFormatado()
        {
            if (PercentualCapital.HasValue)
                return $"{PercentualCapital.Value:N2}%";

            return "N/A";
        }

        public string GetNomeAtual()
        {
            return Nome ?? NomeSocio ?? string.Empty;
        }

        public string GetQualificacaoAtual()
        {
            return Qualificacao ?? QualificacaoSocio ?? string.Empty;
        }

        public string GetCpfCnpjAtual()
        {
            return CpfCnpj ?? CPFSocio ?? string.Empty;
        }
    }
}
