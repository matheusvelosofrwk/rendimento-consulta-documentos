namespace ConsultaDocumentos.Application.DTOs
{
    public class DocumentoDTO : BaseDTO
    {
        // Campos principais
        public char TipoPessoa { get; set; }
        public string Numero { get; set; }
        public string Nome { get; set; }
        public DateTime DataConsulta { get; set; }
        public DateTime DataConsultaValidade { get; set; }

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
        public string? Porte { get; set; }

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
    }
}
