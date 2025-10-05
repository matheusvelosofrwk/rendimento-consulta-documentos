using ConsultaDocumentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultaDocumentos.Infra.Data.EntitiesConfiguration
{
    public class TelefoneConfiguration : IEntityTypeConfiguration<Telefone>
    {
        public void Configure(EntityTypeBuilder<Telefone> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.IdDocumento)
                .IsRequired();

            builder.Property(x => x.DDD)
                .HasMaxLength(3);

            builder.Property(x => x.Numero)
                .HasMaxLength(15);

            builder.Property(x => x.Tipo)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.DataCriacao)
                .IsRequired();

            // Relacionamento
            builder.HasOne(t => t.Documento)
                .WithMany(d => d.Telefones)
                .HasForeignKey(t => t.IdDocumento)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(x => x.IdDocumento);

            builder.HasIndex(x => x.DDD);
        }
    }
}
