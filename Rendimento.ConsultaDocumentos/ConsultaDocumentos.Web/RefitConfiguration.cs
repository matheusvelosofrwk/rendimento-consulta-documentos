using ConsultaDocumentos.Web.Clients;
using Refit;

namespace ConsultaDocumentos.Web
{
    public static class RefitConfiguration
    {
        public static IServiceCollection AddRefitConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // ler a BaseUrl do appsettings
            var baseUrl = configuration["ApiClients:JsonPlaceholder:BaseUrl"] ?? throw new InvalidOperationException("BaseUrl não configurada");

            // registrar o cliente Refit tipado
            services
                .AddRefitClient<IClienteApi>(new RefitSettings
                {
                    // Refit já usa System.Text.Json por padrão; customize aqui se quiser
                    // ContentSerializer = new SystemTextJsonContentSerializer(customOptions)
                })
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
                // (opcional) resiliência nativa do .NET 8
                .AddStandardResilienceHandler(o =>
                {
                    o.Retry.MaxRetryAttempts = 3;
                    o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
                    // Circuit breaker e rate limiter já vêm com defaults sólidos
                });

            services
                .AddRefitClient<IAplicacaoApi>(new RefitSettings
                {
                    // Refit já usa System.Text.Json por padrão; customize aqui se quiser
                    // ContentSerializer = new SystemTextJsonContentSerializer(customOptions)
                })
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
                // (opcional) resiliência nativa do .NET 8
                .AddStandardResilienceHandler(o =>
                {
                    o.Retry.MaxRetryAttempts = 3;
                    o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
                    // Circuit breaker e rate limiter já vêm com defaults sólidos
                });


            return services;
        }
    }
}
