using ConsultaDocumentos.Web.Handlers;
using ConsultaDocumentos.Web.Services.Http;

namespace ConsultaDocumentos.Web
{
    public static class HttpClientConfiguration
    {
        public static IServiceCollection AddHttpClientConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Ler a BaseUrl do appsettings
            var baseUrl = configuration["ApiClients:JsonPlaceholder:BaseUrl"] ?? throw new InvalidOperationException("BaseUrl não configurada");

            // Garantir que a BaseUrl termine com '/' para que o HttpClient concatene corretamente os endpoints
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            // Registrar o AuthDelegatingHandler
            services.AddTransient<AuthDelegatingHandler>();

            // Configurar HttpClient para AuthHttpService (sem AuthDelegatingHandler)
            services.AddHttpClient<AuthHttpService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            });

            // Configurar HttpClient para AplicacaoHttpService (com AuthDelegatingHandler)
            services.AddHttpClient<AplicacaoHttpService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddHttpMessageHandler<AuthDelegatingHandler>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            });

            // Configurar HttpClient para ProvedorHttpService (com AuthDelegatingHandler)
            services.AddHttpClient<ProvedorHttpService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddHttpMessageHandler<AuthDelegatingHandler>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            });

            // Configurar HttpClient para NacionalidadeHttpService (com AuthDelegatingHandler)
            services.AddHttpClient<NacionalidadeHttpService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddHttpMessageHandler<AuthDelegatingHandler>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            });

            // Configurar HttpClient para SituacaoCadastralHttpService (com AuthDelegatingHandler)
            services.AddHttpClient<SituacaoCadastralHttpService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddHttpMessageHandler<AuthDelegatingHandler>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            });

            // Configurar HttpClient para DocumentoHttpService (com AuthDelegatingHandler)
            services.AddHttpClient<DocumentoHttpService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddHttpMessageHandler<AuthDelegatingHandler>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            });

            // Configurar HttpClient para AplicacaoProvedorHttpService (com AuthDelegatingHandler)
            services.AddHttpClient<AplicacaoProvedorHttpService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddHttpMessageHandler<AuthDelegatingHandler>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            });

            // Configurar HttpClient para LogAuditoriaHttpService (com AuthDelegatingHandler)
            services.AddHttpClient<LogAuditoriaHttpService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddHttpMessageHandler<AuthDelegatingHandler>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            });

            // Configurar HttpClient para LogErroHttpService (com AuthDelegatingHandler)
            services.AddHttpClient<LogErroHttpService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddHttpMessageHandler<AuthDelegatingHandler>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            });

            // Configurar HttpClient para RoleHttpService (com AuthDelegatingHandler)
            services.AddHttpClient<RoleHttpService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddHttpMessageHandler<AuthDelegatingHandler>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            });

            // Configurar HttpClient para UserHttpService (com AuthDelegatingHandler)
            services.AddHttpClient<UserHttpService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddHttpMessageHandler<AuthDelegatingHandler>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            });

            // Configurar HttpClient para QuadroSocietarioHttpService (com AuthDelegatingHandler)
            services.AddHttpClient<QuadroSocietarioHttpService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddHttpMessageHandler<AuthDelegatingHandler>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            });

            return services;
        }
    }
}
