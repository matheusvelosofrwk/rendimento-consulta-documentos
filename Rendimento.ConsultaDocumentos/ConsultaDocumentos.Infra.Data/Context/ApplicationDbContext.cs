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

        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
