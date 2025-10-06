using ConsultaDocumentos.API.HealthChecks;
using ConsultaDocumentos.Infra.Data.Context;
using ConsultaDocumentos.Infra.Ioc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
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

// Aplicar migrations automaticamente e fazer seed de dados
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Aplicando migrations...");
        context.Database.Migrate();
        logger.LogInformation("Migrations aplicadas com sucesso!");

        // Seed de dados inicial (Identity roles e usuário admin)
        var userManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<Microsoft.AspNetCore.Identity.IdentityUser>>();
        var roleManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();

        await SeedData(userManager, roleManager, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao aplicar migrations ou fazer seed de dados");
    }
}

static async Task SeedData(
    Microsoft.AspNetCore.Identity.UserManager<Microsoft.AspNetCore.Identity.IdentityUser> userManager,
    Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole> roleManager,
    ILogger logger)
{
    // Criar roles padrão
    string[] roles = { "Admin", "User", "Manager" };
    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(roleName));
            logger.LogInformation($"Role '{roleName}' criada");
        }
    }

    // Criar usuário admin padrão
    var adminEmail = "admin@consultadocumentos.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new Microsoft.AspNetCore.Identity.IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
            logger.LogInformation($"Usuário admin criado: {adminEmail}");
        }
        else
        {
            logger.LogError($"Erro ao criar usuário admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}

// Configure the HTTP request pipeline.
// Habilitar Swagger em todos os ambientes (Development e Production)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ConsultaDocumentos API v1");
    c.RoutePrefix = "swagger"; // Acessível em: /swagger
});

// Comentado pois causa problemas no Docker onde não temos HTTPS configurado
// app.UseHttpsRedirection();

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
