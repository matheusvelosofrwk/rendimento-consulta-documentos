using ConsultaDocumentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultaDocumentos.Infra.Data.EntitiesConfiguration
{
    public class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.IdDocumento)
                .IsRequired();

            builder.Property(x => x.Logradouro)
                .HasMaxLength(200);

            builder.Property(x => x.Numero)
                .HasMaxLength(20);

            builder.Property(x => x.Complemento)
                .HasMaxLength(100);

            builder.Property(x => x.Bairro)
                .HasMaxLength(100);

            builder.Property(x => x.CEP)
                .HasMaxLength(10);

            builder.Property(x => x.Cidade)
                .HasMaxLength(100);

            builder.Property(x => x.UF)
                .HasMaxLength(2);

            builder.Property(x => x.Tipo)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.TipoLogradouro)
                .HasMaxLength(50);

            // Relacionamento
            builder.HasOne(e => e.Documento)
                .WithMany(d => d.Enderecos)
                .HasForeignKey(e => e.IdDocumento)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(x => x.IdDocumento);

            builder.HasIndex(x => x.CEP);

            builder.HasIndex(e => new { e.Cidade, e.UF });
        }
    }
}
