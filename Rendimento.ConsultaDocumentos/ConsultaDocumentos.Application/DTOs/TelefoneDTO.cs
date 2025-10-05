using ConsultaDocumentos.Domain.Enums;

namespace ConsultaDocumentos.Application.DTOs
{
    public class TelefoneDTO : BaseDTO
    {
        public Guid IdDocumento { get; set; }
        public string? DDD { get; set; }
        public string? Numero { get; set; }
        public TipoTelefone Tipo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}
