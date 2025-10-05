using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Application.Services;
using ConsultaDocumentos.Domain.Intefaces;
using ConsultaDocumentos.Infra.Data.Context;
using ConsultaDocumentos.Infra.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ConsultaDocumentos.Infra.Ioc
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            });

            // Configurar Identity
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // Configurações de senha
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                // Configurações de usuário
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Configurar JWT Authentication
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });

            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IAplicacaoRepository, AplicacaoRepository>();
            services.AddScoped<IProvedorRepository, ProvedorRepository>();
            services.AddScoped<INacionalidadeRepository, NacionalidadeRepository>();
            services.AddScoped<ISituacaoCadastralRepository, SituacaoCadastralRepository>();
            services.AddScoped<IDocumentoRepository, DocumentoRepository>();
            services.AddScoped<IEnderecoRepository, EnderecoRepository>();
            services.AddScoped<ITelefoneRepository, TelefoneRepository>();
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IQuadroSocietarioRepository, QuadroSocietarioRepository>();
            services.AddScoped<IAplicacaoProvedorRepository, AplicacaoProvedorRepository>();
            services.AddScoped<ILogAuditoriaRepository, LogAuditoriaRepository>();
            services.AddScoped<ILogErroRepository, LogErroRepository>();

            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IAplicacaoService, AplicacaoService>();
            services.AddScoped<IProvedorService, ProvedorService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<INacionalidadeService, NacionalidadeService>();
            services.AddScoped<ISituacaoCadastralService, SituacaoCadastralService>();
            services.AddScoped<IDocumentoService, DocumentoService>();
            services.AddScoped<IEnderecoService, EnderecoService>();
            services.AddScoped<ITelefoneService, TelefoneService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IQuadroSocietarioService, QuadroSocietarioService>();
            services.AddScoped<IAplicacaoProvedorService, AplicacaoProvedorService>();

            return services;
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Application.Mappings.DomainToDTOMappingProfile));

            return services;
        }
    }
}
