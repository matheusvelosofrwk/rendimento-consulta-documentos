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

            builder.Property(x => x.DataCriacao)
                .IsRequired();
        }
    }
}
