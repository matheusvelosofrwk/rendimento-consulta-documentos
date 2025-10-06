namespace ConsultaDocumentos.Application.DTOs
{
    public class ProvedorDTO : BaseDTO
    {
        public string Credencial { get; set; }

        public string Descricao { get; set; }

        public string EndpointUrl { get; set; }

        public string Nome { get; set; }

        public int Prioridade { get; set; }

        public string Status { get; set; }

        public int Timeout { get; set; }

        // Novos campos
        public string? EndCertificado { get; set; }
        public string? Usuario { get; set; }
        public string? Senha { get; set; }
        public string? Dominio { get; set; }
        public int? QtdAcessoMinimo { get; set; }
        public int? QtdAcessoMaximo { get; set; }
        public int QtdDiasValidadePF { get; set; }
        public int QtdDiasValidadePJ { get; set; }
        public int QtdDiasValidadeEND { get; set; }
        public int? QtdMinEmailLog { get; set; }
        public int? DiaCorte { get; set; }
        public int? Porta { get; set; }
        public int TipoWebService { get; set; }
    }
}
