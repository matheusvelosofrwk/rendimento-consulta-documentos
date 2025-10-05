using ConsultaDocumentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultaDocumentos.Infra.Data.EntitiesConfiguration
{
    public class LogErroConfiguration : IEntityTypeConfiguration<LogErro>
    {
        public void Configure(EntityTypeBuilder<LogErro> builder)
        {
            builder.HasKey(x => x.Id);

            // Campos
            builder.Property(x => x.DataHora)
                .IsRequired();

            builder.Property(x => x.Aplicacao)
                .HasMaxLength(200);

            builder.Property(x => x.Metodo)
                .HasMaxLength(500);

            builder.Property(x => x.Erro)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.StackTrace)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.Usuario)
                .HasMaxLength(256); // IdentityUser.Id é string de até 256

            builder.Property(x => x.IdSistema);

            // Relacionamentos
            builder.HasOne(x => x.Sistema)
                .WithMany()
                .HasForeignKey(x => x.IdSistema)
                .OnDelete(DeleteBehavior.SetNull);

            // Índices otimizados para consultas
            builder.HasIndex(x => x.DataHora);
            builder.HasIndex(x => x.Aplicacao);
            builder.HasIndex(x => x.Usuario);
            builder.HasIndex(x => x.IdSistema);
        }
    }
}
