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

            builder.Property(x => x.Serpro)
                .IsRequired()
                .HasDefaultValue(false);

            // Configurações do Certificado Digital
            builder.Property(x => x.CertificadoCaminho)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(x => x.CertificadoSenha)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(x => x.CertificadoSenhaCriptografada)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(x => x.CertificadoDataExpiracao)
                .IsRequired(false);
        }
    }
}
