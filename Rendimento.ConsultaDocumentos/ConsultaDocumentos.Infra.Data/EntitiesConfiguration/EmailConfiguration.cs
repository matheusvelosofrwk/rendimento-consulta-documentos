using ConsultaDocumentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultaDocumentos.Infra.Data.EntitiesConfiguration
{
    public class EmailConfiguration : IEntityTypeConfiguration<Email>
    {
        public void Configure(EntityTypeBuilder<Email> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.IdDocumento)
                .IsRequired();

            builder.Property(x => x.EnderecoEmail)
                .HasMaxLength(200);

            builder.Property(x => x.DataCriacao)
                .IsRequired();

            // Relacionamento
            builder.HasOne(e => e.Documento)
                .WithMany(d => d.Emails)
                .HasForeignKey(e => e.IdDocumento)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(x => x.IdDocumento);

            builder.HasIndex(x => x.EnderecoEmail);
        }
    }
}
