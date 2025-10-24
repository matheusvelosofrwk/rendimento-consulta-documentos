namespace ConsultaDocumentos.Application.DTOs
{
    public class NacionalidadeDTO : BaseDTO
    {
        public string Descricao { get; set; }

        public string? Pais { get; set; }

        public string? Codigo { get; set; }

        public bool Ativo { get; set; }
    }
}
