using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ConsultaDocumentos.Infra.Data.EntitiesConfiguration
{
    public class AplicacaoProvedorConfiguration : IEntityTypeConfiguration<AplicacaoProvedor>
    {
        public void Configure(EntityTypeBuilder<AplicacaoProvedor> builder)
        {
            builder.HasKey(x => x.Id);

            // Propriedades
            builder.Property(x => x.AplicacaoId)
                .IsRequired();

            builder.Property(x => x.ProvedorId)
                .IsRequired();

            builder.Property(x => x.Ordem)
                .IsRequired();

            // Converter explicitamente o enum StatusEnum para char
            builder.Property(x => x.Status)
                .HasConversion(
                    v => (char)v,
                    v => (StatusEnum)v)
                .HasColumnType("char(1)")
                .IsRequired();

            builder.Property(x => x.DataCriacao)
                .IsRequired();

            builder.Property(x => x.DataAtualizacao);

            builder.Property(x => x.CriadoPor)
                .HasMaxLength(450); // Tamanho padrão do IdentityUser.Id

            builder.Property(x => x.AtualizadoPor)
                .HasMaxLength(450);

            // Campos de LOG DE USO
            builder.Property(x => x.DataConsulta);

            builder.Property(x => x.EnderecoIP)
                .HasMaxLength(50);

            builder.Property(x => x.RemoteHost)
                .HasMaxLength(100);

            // Relacionamentos
            builder.HasOne(x => x.Aplicacao)
                .WithMany()
                .HasForeignKey(x => x.AplicacaoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Provedor)
                .WithMany()
                .HasForeignKey(x => x.ProvedorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Documento)
                .WithMany()
                .HasForeignKey(x => x.IdDocumento)
                .OnDelete(DeleteBehavior.SetNull);

            // Índices
            builder.HasIndex(x => new { x.AplicacaoId, x.ProvedorId })
                .IsUnique()
                .HasDatabaseName("IX_AplicacaoProvedor_Aplicacao_Provedor");

            builder.HasIndex(x => x.Ordem)
                .HasDatabaseName("IX_AplicacaoProvedor_Ordem");

            builder.HasIndex(x => x.AplicacaoId)
                .HasDatabaseName("IX_AplicacaoProvedor_AplicacaoId");

            builder.HasIndex(x => x.ProvedorId)
                .HasDatabaseName("IX_AplicacaoProvedor_ProvedorId");

            // Índices para billing/relatórios
            builder.HasIndex(x => x.DataConsulta)
                .HasDatabaseName("IX_AplicacaoProvedor_DataConsulta");

            builder.HasIndex(x => x.IdDocumento)
                .HasDatabaseName("IX_AplicacaoProvedor_IdDocumento");

            builder.HasIndex(x => new { x.AplicacaoId, x.ProvedorId, x.DataConsulta })
                .HasDatabaseName("IX_AplicacaoProvedor_Billing");
        }
    }
}
