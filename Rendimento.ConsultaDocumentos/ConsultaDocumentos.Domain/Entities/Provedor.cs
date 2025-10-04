using ConsultaDocumentos.Domain.Base;

namespace ConsultaDocumentos.Domain.Entities
{
    public class Provedor : BaseEntity
    {
        public string Credencial { get; set; }

        public string Descricao { get; set; }

        public string EndpointUrl { get; set; }

        public string Nome { get; set; }

        public int Prioridade { get; set; }

        public string Status { get; set; }

        public int Timeout { get; set; }
    }
}
