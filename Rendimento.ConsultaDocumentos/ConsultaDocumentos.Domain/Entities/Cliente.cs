using ConsultaDocumentos.Domain.Base;

namespace ConsultaDocumentos.Domain.Entities
{
    public class Cliente : BaseEntity
    {
        public string Nome { get; set; }

        public string CPF { get; set; }

        public string Endereco { get; set; }
    }
}
