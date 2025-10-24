using ConsultaDocumentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultaDocumentos.Infra.Data.EntitiesConfiguration
{
    public class NacionalidadeConfiguration : IEntityTypeConfiguration<Nacionalidade>
    {
        public void Configure(EntityTypeBuilder<Nacionalidade> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Descricao)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Pais)
                .HasMaxLength(100);

            builder.Property(x => x.Codigo)
                .HasMaxLength(10);

            builder.Property(x => x.Ativo)
                .IsRequired();
        }
    }
}
