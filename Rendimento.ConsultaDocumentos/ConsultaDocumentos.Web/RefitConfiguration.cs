using ConsultaDocumentos.Web.Clients;
using ConsultaDocumentos.Web.Handlers;
using Refit;

namespace ConsultaDocumentos.Web
{
    public static class RefitConfiguration
    {
        public static IServiceCollection AddRefitConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // ler a BaseUrl do appsettings
            var baseUrl = configuration["ApiClients:JsonPlaceholder:BaseUrl"] ?? throw new InvalidOperationException("BaseUrl não configurada");

            // Registrar o AuthDelegatingHandler
            services.AddTransient<AuthDelegatingHandler>();

            // registrar o cliente de autenticação (sem AuthDelegatingHandler)
            services
                .AddRefitClient<IAuthApi>(new RefitSettings
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

            // registrar o cliente Refit tipado com AuthDelegatingHandler
            services
                .AddRefitClient<IClienteApi>(new RefitSettings
                {
                    // Refit já usa System.Text.Json por padrão; customize aqui se quiser
                    // ContentSerializer = new SystemTextJsonContentSerializer(customOptions)
                })
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
                .AddHttpMessageHandler<AuthDelegatingHandler>()
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
                .AddHttpMessageHandler<AuthDelegatingHandler>()
                // (opcional) resiliência nativa do .NET 8
                .AddStandardResilienceHandler(o =>
                {
                    o.Retry.MaxRetryAttempts = 3;
                    o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
                    // Circuit breaker e rate limiter já vêm com defaults sólidos
                });

            services
                .AddRefitClient<IProvedorApi>(new RefitSettings
                {
                    // Refit já usa System.Text.Json por padrão; customize aqui se quiser
                    // ContentSerializer = new SystemTextJsonContentSerializer(customOptions)
                })
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
                .AddHttpMessageHandler<AuthDelegatingHandler>()
                // (opcional) resiliência nativa do .NET 8
                .AddStandardResilienceHandler(o =>
                {
                    o.Retry.MaxRetryAttempts = 3;
                    o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
                    // Circuit breaker e rate limiter já vêm com defaults sólidos
                });

            services
                .AddRefitClient<INacionalidadeApi>(new RefitSettings
                {
                    // Refit já usa System.Text.Json por padrão; customize aqui se quiser
                    // ContentSerializer = new SystemTextJsonContentSerializer(customOptions)
                })
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
                .AddHttpMessageHandler<AuthDelegatingHandler>()
                // (opcional) resiliência nativa do .NET 8
                .AddStandardResilienceHandler(o =>
                {
                    o.Retry.MaxRetryAttempts = 3;
                    o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
                    // Circuit breaker e rate limiter já vêm com defaults sólidos
                });

            services
                .AddRefitClient<ISituacaoCadastralApi>(new RefitSettings
                {
                    // Refit já usa System.Text.Json por padrão; customize aqui se quiser
                    // ContentSerializer = new SystemTextJsonContentSerializer(customOptions)
                })
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
                .AddHttpMessageHandler<AuthDelegatingHandler>()
                // (opcional) resiliência nativa do .NET 8
                .AddStandardResilienceHandler(o =>
                {
                    o.Retry.MaxRetryAttempts = 3;
                    o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
                    // Circuit breaker e rate limiter já vêm com defaults sólidos
                });

            services
                .AddRefitClient<IDocumentoApi>(new RefitSettings
                {
                    // Refit já usa System.Text.Json por padrão; customize aqui se quiser
                    // ContentSerializer = new SystemTextJsonContentSerializer(customOptions)
                })
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
                .AddHttpMessageHandler<AuthDelegatingHandler>()
                // (opcional) resiliência nativa do .NET 8
                .AddStandardResilienceHandler(o =>
                {
                    o.Retry.MaxRetryAttempts = 3;
                    o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
                    // Circuit breaker e rate limiter já vêm com defaults sólidos
                });

            services
                .AddRefitClient<IAplicacaoProvedorApi>(new RefitSettings
                {
                    // Refit já usa System.Text.Json por padrão; customize aqui se quiser
                    // ContentSerializer = new SystemTextJsonContentSerializer(customOptions)
                })
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
                .AddHttpMessageHandler<AuthDelegatingHandler>()
                // (opcional) resiliência nativa do .NET 8
                .AddStandardResilienceHandler(o =>
                {
                    o.Retry.MaxRetryAttempts = 3;
                    o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
                    // Circuit breaker e rate limiter já vêm com defaults sólidos
                });

            services
                .AddRefitClient<ILogAuditoriaApi>(new RefitSettings
                {
                    // Refit já usa System.Text.Json por padrão; customize aqui se quiser
                    // ContentSerializer = new SystemTextJsonContentSerializer(customOptions)
                })
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
                .AddHttpMessageHandler<AuthDelegatingHandler>()
                // (opcional) resiliência nativa do .NET 8
                .AddStandardResilienceHandler(o =>
                {
                    o.Retry.MaxRetryAttempts = 3;
                    o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
                    // Circuit breaker e rate limiter já vêm com defaults sólidos
                });

            services
                .AddRefitClient<ILogErroApi>(new RefitSettings
                {
                    // Refit já usa System.Text.Json por padrão; customize aqui se quiser
                    // ContentSerializer = new SystemTextJsonContentSerializer(customOptions)
                })
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
                .AddHttpMessageHandler<AuthDelegatingHandler>()
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
