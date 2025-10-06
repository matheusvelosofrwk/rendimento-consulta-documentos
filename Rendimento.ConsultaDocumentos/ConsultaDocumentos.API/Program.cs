using ConsultaDocumentos.API.HealthChecks;
using ConsultaDocumentos.Infra.Data.Context;
using ConsultaDocumentos.Infra.Ioc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ConsultaDocumentos API", Version = "v1" });

    // Configurar autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer scheme. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configurar Health Checks
var healthChecksBuilder = builder.Services.AddHealthChecks();

// Health check do banco de dados SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString))
{
    healthChecksBuilder
        .AddDbContextCheck<ApplicationDbContext>(
            name: "database",
            tags: new[] { "db", "sql", "ready" })
        .AddSqlServer(
            connectionString: connectionString,
            name: "sqlserver",
            tags: new[] { "db", "sql", "ready" });
}

// Health check do Redis (se habilitado)
var redisEnabled = builder.Configuration.GetValue<bool>("Redis:Enabled");
var redisConnectionString = builder.Configuration.GetValue<string>("Redis:ConnectionString");
if (redisEnabled && !string.IsNullOrEmpty(redisConnectionString))
{
    healthChecksBuilder.AddRedis(
        redisConnectionString: redisConnectionString,
        name: "redis",
        tags: new[] { "cache", "redis", "ready" });
}

// Health check dos provedores externos
healthChecksBuilder.AddCheck<ExternalProvidersHealthCheck>(
    name: "external_providers",
    tags: new[] { "external", "providers", "ready" });

// Adicionar HttpClient para uso no health check de provedores (opcional)
builder.Services.AddHttpClient();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAutoMapper();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Configurar endpoints de Health Check
// Endpoint básico para liveness probe (apenas verifica se a aplicação está rodando)
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false, // Não executa nenhum check específico
    AllowCachingResponses = false
});

// Endpoint detalhado para readiness probe (verifica todos os health checks)
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    AllowCachingResponses = false
});

// Endpoint com todos os health checks em formato JSON detalhado
app.MapHealthChecks("/health/detailed", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    AllowCachingResponses = false
});

app.Run();
