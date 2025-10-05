using ConsultaDocumentos.Domain.Base;
using ConsultaDocumentos.Domain.Enums;

namespace ConsultaDocumentos.Domain.Entities
{
    public class Endereco : BaseEntity
    {
        public Guid IdDocumento { get; set; }
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? CEP { get; set; }
        public string? Cidade { get; set; }
        public string? UF { get; set; }
        public TipoEndereco Tipo { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public byte[]? RowVersion { get; set; }
        public string? TipoLogradouro { get; set; }

        // Navigation Property
        public virtual Documento? Documento { get; set; }

        // Factory Method
        public static Endereco Criar(
            Guid idDocumento,
            string? logradouro,
            string? numero,
            string? bairro,
            string? cep,
            string? cidade,
            string? uf,
            TipoEndereco tipo)
        {
            return new Endereco
            {
                Id = Guid.NewGuid(),
                IdDocumento = idDocumento,
                Logradouro = logradouro,
                Numero = numero,
                Bairro = bairro,
                CEP = cep,
                Cidade = cidade,
                UF = uf?.ToUpper(),
                Tipo = tipo,
                DataAtualizacao = DateTime.UtcNow
            };
        }

        // Métodos de Domínio
        public bool IsValido()
        {
            return !string.IsNullOrWhiteSpace(Logradouro) ||
                   !string.IsNullOrWhiteSpace(CEP) ||
                   !string.IsNullOrWhiteSpace(Cidade);
        }

        public bool IsCompleto()
        {
            return !string.IsNullOrWhiteSpace(Logradouro) &&
                   !string.IsNullOrWhiteSpace(Numero) &&
                   !string.IsNullOrWhiteSpace(Bairro) &&
                   !string.IsNullOrWhiteSpace(CEP) &&
                   !string.IsNullOrWhiteSpace(Cidade) &&
                   !string.IsNullOrWhiteSpace(UF);
        }

        public bool IsCepValido()
        {
            return !string.IsNullOrWhiteSpace(CEP) && CEP.Replace("-", "").Length == 8;
        }

        public string GetEnderecoFormatado()
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(Logradouro))
                parts.Add(Logradouro);

            if (!string.IsNullOrWhiteSpace(Numero))
                parts.Add(Numero);

            if (!string.IsNullOrWhiteSpace(Complemento))
                parts.Add(Complemento);

            if (!string.IsNullOrWhiteSpace(Bairro))
                parts.Add(Bairro);

            if (!string.IsNullOrWhiteSpace(Cidade) && !string.IsNullOrWhiteSpace(UF))
                parts.Add($"{Cidade}/{UF}");

            if (!string.IsNullOrWhiteSpace(CEP))
                parts.Add($"CEP: {CEP}");

            return string.Join(", ", parts);
        }

        public void Atualizar(
            string? logradouro,
            string? numero,
            string? complemento,
            string? bairro,
            string? cep,
            string? cidade,
            string? uf)
        {
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            CEP = cep;
            Cidade = cidade;
            UF = uf?.ToUpper();
            DataAtualizacao = DateTime.UtcNow;
        }
    }
}
