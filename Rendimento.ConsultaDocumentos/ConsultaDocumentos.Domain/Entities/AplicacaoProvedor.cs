using ConsultaDocumentos.Domain.Base;
using ConsultaDocumentos.Domain.Enums;

namespace ConsultaDocumentos.Domain.Entities
{
    public class AplicacaoProvedor : BaseEntity
    {
        public Guid AplicacaoId { get; private set; }
        public Guid ProvedorId { get; private set; }
        public int Ordem { get; private set; }
        public StatusEnum Status { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataAtualizacao { get; private set; }
        public string? CriadoPor { get; private set; }
        public string? AtualizadoPor { get; private set; }

        // Navigation Properties
        public virtual Aplicacao Aplicacao { get; set; }
        public virtual Provedor Provedor { get; set; }

        // Construtor privado para EF Core
        private AplicacaoProvedor() { }

        // Factory Method
        public static AplicacaoProvedor Criar(Guid aplicacaoId, Guid provedorId, int ordem, string criadoPor)
        {
            ValidarOrdem(ordem);

            return new AplicacaoProvedor
            {
                Id = Guid.NewGuid(),
                AplicacaoId = aplicacaoId,
                ProvedorId = provedorId,
                Ordem = ordem,
                Status = StatusEnum.Ativo,
                DataCriacao = DateTime.Now,
                CriadoPor = criadoPor
            };
        }

        // Métodos de Domínio
        public void AtualizarOrdem(int novaOrdem, string atualizadoPor)
        {
            ValidarOrdem(novaOrdem);

            Ordem = novaOrdem;
            DataAtualizacao = DateTime.Now;
            AtualizadoPor = atualizadoPor;
        }

        public void Ativar(string atualizadoPor)
        {
            Status = StatusEnum.Ativo;
            DataAtualizacao = DateTime.Now;
            AtualizadoPor = atualizadoPor;
        }

        public void Desativar(string atualizadoPor)
        {
            Status = StatusEnum.Inativo;
            DataAtualizacao = DateTime.Now;
            AtualizadoPor = atualizadoPor;
        }

        // Validações
        private static void ValidarOrdem(int ordem)
        {
            if (ordem <= 0)
            {
                throw new ArgumentException("A ordem deve ser maior que zero.", nameof(ordem));
            }
        }

        public bool IsAtivo()
        {
            return Status == StatusEnum.Ativo;
        }
    }
}
