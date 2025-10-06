using ConsultaDocumentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultaDocumentos.Infra.Data.EntitiesConfiguration
{
    public class QuadroSocietarioConfiguration : IEntityTypeConfiguration<QuadroSocietario>
    {
        public void Configure(EntityTypeBuilder<QuadroSocietario> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.IdDocumento)
                .IsRequired();

            // Campos legados
            builder.Property(x => x.CPFSocio)
                .HasMaxLength(20);

            builder.Property(x => x.NomeSocio)
                .HasMaxLength(200);

            builder.Property(x => x.QualificacaoSocio)
                .HasMaxLength(100);

            // Campos novos
            builder.Property(x => x.CpfCnpj)
                .HasMaxLength(20);

            builder.Property(x => x.Nome)
                .HasMaxLength(200);

            builder.Property(x => x.Qualificacao)
                .HasMaxLength(100);

            // Representante legal
            builder.Property(x => x.CpfRepresentanteLegal)
                .HasMaxLength(20);

            builder.Property(x => x.NomeRepresentanteLegal)
                .HasMaxLength(200);

            builder.Property(x => x.QualificacaoRepresentanteLegal)
                .HasMaxLength(100);

            // Percentual
            builder.Property(x => x.PercentualCapital)
                .HasPrecision(5, 2);

            builder.Property(x => x.DataCriacao)
                .IsRequired();

            // Novos campos
            builder.Property(x => x.Tipo)
                .HasMaxLength(100);

            builder.Property(x => x.CodPaisOrigem)
                .HasMaxLength(10);

            builder.Property(x => x.NomePaisOrigem)
                .HasMaxLength(100);

            // Relacionamentos
            builder.HasOne(q => q.Documento)
                .WithMany(d => d.QuadrosSocietarios)
                .HasForeignKey(q => q.IdDocumento)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Nacionalidade)
                .WithMany()
                .HasForeignKey(x => x.IdNacionalidade)
                .OnDelete(DeleteBehavior.SetNull);

            // Índices
            builder.HasIndex(x => x.IdDocumento);

            builder.HasIndex(x => x.IdNacionalidade)
                .HasDatabaseName("IX_QuadroSocietario_IdNacionalidade");
        }
    }
}
