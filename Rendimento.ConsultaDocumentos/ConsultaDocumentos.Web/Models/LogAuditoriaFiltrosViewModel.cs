namespace ConsultaDocumentos.Web.Models
{
    public class LogAuditoriaFiltrosViewModel
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string? NumeroDocumento { get; set; }
        public Guid? AplicacaoProvedorId { get; set; }
        public int? TipoDocumento { get; set; }
    }
}
