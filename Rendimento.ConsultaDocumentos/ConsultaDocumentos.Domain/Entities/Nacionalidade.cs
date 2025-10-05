using ConsultaDocumentos.Domain.Base;

namespace ConsultaDocumentos.Domain.Entities
{
    public class Nacionalidade : BaseEntity
    {
        public string Descricao { get; set; }

        public string? Codigo { get; set; }

        public bool Ativo { get; set; }
    }
}
