using ConsultaDocumentos.Domain.Base;

namespace ConsultaDocumentos.Domain.Entities
{
    public class Aplicacao : BaseEntity
    {
        public string Nome { get; set; }

        public string Descricao { get; set; }

        public string Status { get; set; }
    }
}
