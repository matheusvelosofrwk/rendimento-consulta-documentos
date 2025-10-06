using ConsultaDocumentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultaDocumentos.Infra.Data.EntitiesConfiguration
{
    public class DocumentoConfiguration : IEntityTypeConfiguration<Documento>
    {
        public void Configure(EntityTypeBuilder<Documento> builder)
        {
            builder.HasKey(x => x.Id);

            // Campos principais
            builder.Property(x => x.TipoPessoa)
                .HasMaxLength(1)
                .IsRequired();

            builder.Property(x => x.Numero)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Nome)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.DataConsulta)
                .IsRequired();

            builder.Property(x => x.DataConsultaValidade)
                .IsRequired();

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            // Campos PJ
            builder.Property(x => x.Inscricao)
                .HasMaxLength(50);

            builder.Property(x => x.DescricaoNaturezaJuridica)
                .HasMaxLength(200);

            builder.Property(x => x.Segmento)
                .HasMaxLength(100);

            builder.Property(x => x.DescricaoRamoAtividade)
                .HasMaxLength(200);

            builder.Property(x => x.NomeFantasia)
                .HasMaxLength(200);

            // Campos PF
            builder.Property(x => x.NomeMae)
                .HasMaxLength(200);

            builder.Property(x => x.Sexo)
                .HasMaxLength(10);

            builder.Property(x => x.TituloEleitor)
                .HasMaxLength(20);

            builder.Property(x => x.NomeSocial)
                .HasMaxLength(100);

            builder.Property(x => x.Porte)
                .HasMaxLength(50);

            // Campos de Situação
            builder.Property(x => x.CodControle)
                .HasMaxLength(50);

            builder.Property(x => x.OrigemBureau)
                .HasMaxLength(100);

            // Relacionamentos
            builder.HasOne(d => d.Nacionalidade)
                .WithMany()
                .HasForeignKey(d => d.IdNacionalidade)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(d => d.SituacaoCadastral)
                .WithMany()
                .HasForeignKey(d => d.IdSituacao)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(d => d.Enderecos)
                .WithOne(e => e.Documento)
                .HasForeignKey(e => e.IdDocumento)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.Telefones)
                .WithOne(t => t.Documento)
                .HasForeignKey(t => t.IdDocumento)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.Emails)
                .WithOne(e => e.Documento)
                .HasForeignKey(e => e.IdDocumento)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.QuadrosSocietarios)
                .WithOne(q => q.Documento)
                .HasForeignKey(q => q.IdDocumento)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(x => x.Numero)
                .IsUnique();

            builder.HasIndex(x => x.DataConsultaValidade);

            builder.HasIndex(x => x.IdNacionalidade);

            builder.HasIndex(x => x.IdSituacao);
        }
    }
}
