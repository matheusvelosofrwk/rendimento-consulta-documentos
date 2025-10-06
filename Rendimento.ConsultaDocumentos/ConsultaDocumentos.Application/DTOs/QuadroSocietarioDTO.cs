namespace ConsultaDocumentos.Application.DTOs
{
    public class QuadroSocietarioDTO : BaseDTO
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

        // Novos campos
        public string? Tipo { get; set; }
        public Guid? IdNacionalidade { get; set; }
        public string? CodPaisOrigem { get; set; }
        public string? NomePaisOrigem { get; set; }

        // Para exibição
        public NacionalidadeDTO? Nacionalidade { get; set; }
    }
}
