using ConsultaDocumentos.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConsultaDocumentos.Infra.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Cliente> Cliente { get; set; }

        public DbSet<Aplicacao> Aplicacao { get; set; }

        public DbSet<Provedor> Provedor { get; set; }

        public DbSet<Nacionalidade> Nacionalidade { get; set; }

        public DbSet<SituacaoCadastral> SituacaoCadastral { get; set; }

        public DbSet<Documento> Documento { get; set; }

        public DbSet<Endereco> Endereco { get; set; }

        public DbSet<Telefone> Telefone { get; set; }

        public DbSet<Email> Email { get; set; }

        public DbSet<QuadroSocietario> QuadroSocietario { get; set; }

        public DbSet<AplicacaoProvedor> AplicacaoProvedor { get; set; }

        public DbSet<LogAuditoria> LogAuditoria { get; set; }

        public DbSet<LogErro> LogErro { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
