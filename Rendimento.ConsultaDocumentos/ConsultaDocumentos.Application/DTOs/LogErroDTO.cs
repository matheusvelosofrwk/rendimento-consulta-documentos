namespace ConsultaDocumentos.Application.DTOs
{
    public class LogErroDTO
    {
        public Guid Id { get; set; }
        public DateTime DataHora { get; set; }
        public string? Aplicacao { get; set; }
        public string? Metodo { get; set; }
        public string? Erro { get; set; }
        public string? StackTrace { get; set; }
        public string? Usuario { get; set; }
        public Guid? IdSistema { get; set; }
    }
}
