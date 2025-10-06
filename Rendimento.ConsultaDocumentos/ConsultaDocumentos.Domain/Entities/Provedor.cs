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

        // Certificado digital (SERPRO)
        public string? EndCertificado { get; set; }

        // Credenciais
        public string? Usuario { get; set; }
        public string? Senha { get; set; } // Criptografada
        public string? Dominio { get; set; }

        // Controle de quota SERPRO
        public int? QtdAcessoMinimo { get; set; }
        public int? QtdAcessoMaximo { get; set; }

        // Validade de cache por tipo de documento
        public int QtdDiasValidadePF { get; set; } = 30;
        public int QtdDiasValidadePJ { get; set; } = 30;
        public int QtdDiasValidadeEND { get; set; } = 30;

        // Alertas e faturamento
        public int? QtdMinEmailLog { get; set; }
        public int? DiaCorte { get; set; }

        // Porta (BoaVista socket - futuro)
        public int? Porta { get; set; }

        // Tipo de documento suportado
        public int TipoWebService { get; set; } = 3; // 1=CPF, 2=CNPJ, 3=Ambos
    }
}
