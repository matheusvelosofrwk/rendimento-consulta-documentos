using ConsultaDocumentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultaDocumentos.Infra.Data.EntitiesConfiguration
{
    public class ProvedorConfiguration : IEntityTypeConfiguration<Provedor>
    {
        public void Configure(EntityTypeBuilder<Provedor> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Credencial).HasMaxLength(500).IsRequired();

            builder.Property(x => x.Descricao).HasMaxLength(500);

            builder.Property(x => x.EndpointUrl).HasMaxLength(500).IsRequired();

            builder.Property(x => x.Nome).HasMaxLength(100).IsRequired();

            builder.Property(x => x.Prioridade).IsRequired();

            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();

            builder.Property(x => x.Timeout).IsRequired();
        }
    }
}
