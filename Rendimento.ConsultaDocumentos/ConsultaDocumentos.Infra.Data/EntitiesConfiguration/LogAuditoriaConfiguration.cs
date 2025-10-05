using ConsultaDocumentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultaDocumentos.Infra.Data.EntitiesConfiguration
{
    public class LogAuditoriaConfiguration : IEntityTypeConfiguration<LogAuditoria>
    {
        public void Configure(EntityTypeBuilder<LogAuditoria> builder)
        {
            builder.HasKey(x => x.Id);

            // Campos principais
            builder.Property(x => x.AplicacaoId)
                .IsRequired();

            builder.Property(x => x.NomeAplicacao)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.DocumentoNumero)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.TipoDocumento)
                .HasConversion<int>()
                .IsRequired();

            // Parâmetros e resultado (JSON)
            builder.Property(x => x.ParametrosEntrada)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.ProvedoresUtilizados)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.ProvedorPrincipal)
                .HasMaxLength(100);

            builder.Property(x => x.ConsultaSucesso)
                .IsRequired();

            builder.Property(x => x.RespostaProvedor)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.MensagemRetorno)
                .HasMaxLength(500);

            // Métricas
            builder.Property(x => x.TempoProcessamentoMs)
                .IsRequired();

            builder.Property(x => x.DataHoraConsulta)
                .IsRequired();

            // Informações de requisição
            builder.Property(x => x.EnderecoIp)
                .HasMaxLength(45); // IPv6

            builder.Property(x => x.UserAgent)
                .HasMaxLength(500);

            builder.Property(x => x.TokenAutenticacao)
                .HasMaxLength(256);

            // Controle
            builder.Property(x => x.OrigemCache)
                .IsRequired();

            builder.Property(x => x.InformacoesAdicionais)
                .HasColumnType("nvarchar(max)");

            // Relacionamentos
            builder.HasOne(x => x.Aplicacao)
                .WithMany()
                .HasForeignKey(x => x.AplicacaoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices otimizados para consultas
            builder.HasIndex(x => x.AplicacaoId);
            builder.HasIndex(x => x.DocumentoNumero);
            builder.HasIndex(x => x.DataHoraConsulta);
            builder.HasIndex(x => x.ConsultaSucesso);
            builder.HasIndex(x => new { x.AplicacaoId, x.DataHoraConsulta });
        }
    }
}
