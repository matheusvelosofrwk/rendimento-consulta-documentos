using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Application.Services;
using ConsultaDocumentos.Application.Services.External;
using ConsultaDocumentos.Domain.Intefaces;
using ConsultaDocumentos.Infra.Data.Context;
using ConsultaDocumentos.Infra.Data.Repositories;
using ConsultaDocumentos.Infra.Data.Services.Cache;
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

            // Configurar Cache (Redis ou Sem Cache)
            var redisConfig = configuration.GetSection("Redis");
            var redisEnabled = redisConfig.GetValue<bool>("Enabled");
            var redisConnectionString = redisConfig.GetValue<string>("ConnectionString");

            if (redisEnabled && !string.IsNullOrEmpty(redisConnectionString))
            {
                // Redis habilitado - usar cache
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnectionString;
                    options.InstanceName = "RendimentoConsultaDocumentos:";
                });
                services.AddScoped<ICacheService, RedisCacheService>();
            }
            else
            {
                // Redis desabilitado - sem cache, consulta sempre nos providers
                services.AddScoped<ICacheService, NoOpCacheService>();
            }

            // Memory Cache (para cache de provedores - 5 minutos)
            services.AddMemoryCache();

            // HttpContextAccessor (para obter IP/Host em logs de billing)
            services.AddHttpContextAccessor();

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
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<INacionalidadeService, NacionalidadeService>();
            services.AddScoped<ISituacaoCadastralService, SituacaoCadastralService>();
            services.AddScoped<IDocumentoService, DocumentoService>();
            services.AddScoped<IEnderecoService, EnderecoService>();
            services.AddScoped<ITelefoneService, TelefoneService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IQuadroSocietarioService, QuadroSocietarioService>();
            services.AddScoped<IAplicacaoProvedorService, AplicacaoProvedorService>();

            // Configurar HttpClients para APIs Externas
            // HttpClient para SERPRO
            services.AddHttpClient<ISerproService, SerproService>((serviceProvider, client) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                var timeout = config.GetValue<int?>("ExternalProviders:Serpro:Timeout") ?? 30000;
                client.Timeout = TimeSpan.FromMilliseconds(timeout);
            });

            // HttpClient para SERASA
            services.AddHttpClient<ISerasaService, SerasaService>((serviceProvider, client) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                var timeout = config.GetValue<int?>("ExternalProviders:Serasa:Timeout") ?? 30000;
                client.Timeout = TimeSpan.FromMilliseconds(timeout);
            });

            // Serviços de Integração Externa
            services.AddScoped<IExternalDocumentConsultaService, ExternalDocumentConsultaService>();
            services.AddScoped<IProviderHealthCheckService, ProviderHealthCheckService>();

            return services;
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Application.Mappings.DomainToDTOMappingProfile));

            return services;
        }
    }
}
