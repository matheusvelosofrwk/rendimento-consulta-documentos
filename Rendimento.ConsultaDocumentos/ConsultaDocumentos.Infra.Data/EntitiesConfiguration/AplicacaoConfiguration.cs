using ConsultaDocumentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultaDocumentos.Infra.Data.EntitiesConfiguration
{
    public class AplicacaoConfiguration : IEntityTypeConfiguration<Aplicacao>
    {
        public void Configure(EntityTypeBuilder<Aplicacao> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nome).HasMaxLength(100).IsRequired();

            builder.Property(x => x.Descricao).HasMaxLength(500);

            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        }
    }
}
