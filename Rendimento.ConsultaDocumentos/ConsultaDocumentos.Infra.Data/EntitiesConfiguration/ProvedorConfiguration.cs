using ConsultaDocumentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultaDocumentos.Infra.Data.EntitiesConfiguration
{
    public class ProvedorConfiguration : IEntityTypeConfiguration<Provedor>
    {
        public void Configure(EntityTypeBuilder<Provedor> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Credencial).HasMaxLength(500).IsRequired();

            builder.Property(x => x.Descricao).HasMaxLength(500);

            builder.Property(x => x.EndpointUrl).HasMaxLength(500).IsRequired();

            builder.Property(x => x.Nome).HasMaxLength(100).IsRequired();

            builder.Property(x => x.Prioridade).IsRequired();

            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();

            builder.Property(x => x.Timeout).IsRequired();

            // Novos campos
            builder.Property(x => x.EndCertificado)
                .HasMaxLength(500);

            builder.Property(x => x.Usuario)
                .HasMaxLength(100);

            builder.Property(x => x.Senha)
                .HasMaxLength(200); // Criptografada

            builder.Property(x => x.Dominio)
                .HasMaxLength(100);

            builder.Property(x => x.QtdAcessoMinimo);

            builder.Property(x => x.QtdAcessoMaximo);

            builder.Property(x => x.QtdDiasValidadePF)
                .IsRequired()
                .HasDefaultValue(30);

            builder.Property(x => x.QtdDiasValidadePJ)
                .IsRequired()
                .HasDefaultValue(30);

            builder.Property(x => x.QtdDiasValidadeEND)
                .IsRequired()
                .HasDefaultValue(30);

            builder.Property(x => x.QtdMinEmailLog);

            builder.Property(x => x.DiaCorte);

            builder.Property(x => x.Porta);

            builder.Property(x => x.TipoWebService)
                .IsRequired()
                .HasDefaultValue(3);
        }
    }
}
