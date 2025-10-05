using ConsultaDocumentos.Domain.Base;
using System.Text.RegularExpressions;

namespace ConsultaDocumentos.Domain.Entities
{
    public class Email : BaseEntity
    {
        public Guid IdDocumento { get; set; }
        public string? EnderecoEmail { get; set; }
        public DateTime DataCriacao { get; set; }

        // Navigation Property
        public virtual Documento? Documento { get; set; }

        // Factory Method
        public static Email Criar(Guid idDocumento, string? enderecoEmail)
        {
            return new Email
            {
                Id = Guid.NewGuid(),
                IdDocumento = idDocumento,
                EnderecoEmail = enderecoEmail,
                DataCriacao = DateTime.UtcNow
            };
        }

        // Método de Domínio
        public bool IsValido()
        {
            if (string.IsNullOrWhiteSpace(EnderecoEmail))
                return false;

            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(EnderecoEmail, emailPattern);
        }
    }
}
