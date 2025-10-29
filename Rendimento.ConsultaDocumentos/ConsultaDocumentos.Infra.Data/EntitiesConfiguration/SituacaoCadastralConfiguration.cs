using ConsultaDocumentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultaDocumentos.Infra.Data.EntitiesConfiguration
{
    public class SituacaoCadastralConfiguration : IEntityTypeConfiguration<SituacaoCadastral>
    {
        public void Configure(EntityTypeBuilder<SituacaoCadastral> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Descricao)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Ativo)
                .IsRequired();

            builder.Property(x => x.TipoPessoa)
                .HasColumnType("char(1)")
                .IsRequired()
                .HasDefaultValue('A'); // Default: Ambos

            builder.Property(x => x.IdSerpro)
                .HasMaxLength(10)
                .IsRequired(false);

            builder.Property(x => x.DataCriacao)
                .IsRequired();

            // Índice para filtro por tipo de pessoa
            builder.HasIndex(x => x.TipoPessoa);

            // Índice para busca rápida por ID do Serpro
            builder.HasIndex(x => x.IdSerpro);
        }
    }
}
