namespace ConsultaDocumentos.Application.DTOs
{
    public class EmailDTO : BaseDTO
    {
        public Guid IdDocumento { get; set; }
        public string? EnderecoEmail { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
