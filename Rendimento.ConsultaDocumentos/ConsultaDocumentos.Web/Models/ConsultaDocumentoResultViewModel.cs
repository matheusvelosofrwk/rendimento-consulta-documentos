namespace ConsultaDocumentos.Web.Models
{
    public class ConsultaDocumentoResultViewModel
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string? ProvedorUtilizado { get; set; }
        public List<string>? ProvedoresTentados { get; set; }
        public bool OrigemCache { get; set; }
        public long TempoProcessamentoMs { get; set; }
        public DateTime DataConsulta { get; set; }
        public DocumentoViewModel? Documento { get; set; }
        public string? Erro { get; set; }
    }
}
