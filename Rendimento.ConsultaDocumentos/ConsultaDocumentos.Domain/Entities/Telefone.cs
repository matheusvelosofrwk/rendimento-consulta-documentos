using ConsultaDocumentos.Domain.Base;
using ConsultaDocumentos.Domain.Enums;

namespace ConsultaDocumentos.Domain.Entities
{
    public class Telefone : BaseEntity
    {
        public Guid IdDocumento { get; set; }
        public string? DDD { get; set; }
        public string? Numero { get; set; }
        public TipoTelefone Tipo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        // Navigation Property
        public virtual Documento? Documento { get; set; }

        // Factory Method
        public static Telefone Criar(
            Guid idDocumento,
            string? ddd,
            string? numero,
            TipoTelefone tipo)
        {
            return new Telefone
            {
                Id = Guid.NewGuid(),
                IdDocumento = idDocumento,
                DDD = ddd,
                Numero = numero,
                Tipo = tipo,
                DataCriacao = DateTime.UtcNow
            };
        }

        // Métodos de Domínio
        public string GetTelefoneFormatado()
        {
            if (!string.IsNullOrWhiteSpace(DDD) && !string.IsNullOrWhiteSpace(Numero))
                return $"({DDD}) {Numero}";

            return Numero ?? string.Empty;
        }

        public bool IsValido()
        {
            return !string.IsNullOrWhiteSpace(DDD) &&
                   !string.IsNullOrWhiteSpace(Numero) &&
                   DDD.Length >= 2 &&
                   Numero.Length >= 8;
        }

        public bool IsCelular()
        {
            return Tipo == TipoTelefone.Celular;
        }

        public bool IsWhatsApp()
        {
            return Tipo == TipoTelefone.WhatsApp;
        }

        public bool PermiteMensagens()
        {
            return IsCelular() || IsWhatsApp();
        }
    }
}
