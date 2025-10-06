using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ConsultaDocumentos.API.HealthChecks;

public class ExternalProvidersHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory? _httpClientFactory;
    private readonly ILogger<ExternalProvidersHealthCheck> _logger;

    public ExternalProvidersHealthCheck(
        IConfiguration configuration,
        ILogger<ExternalProvidersHealthCheck> logger,
        IHttpClientFactory? httpClientFactory = null)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var useMockProviders = _configuration.GetValue<bool>("ExternalProviders:UseMockProviders");
            var data = new Dictionary<string, object>
            {
                { "UseMockProviders", useMockProviders }
            };

            // Verificar configuração do Serpro
            var serproConfigSection = useMockProviders
                ? "ExternalProviders:Serpro:Mock"
                : "ExternalProviders:Serpro:Real";

            var serproBaseUrl = _configuration.GetValue<string>($"{serproConfigSection}:BaseUrl")
                             ?? _configuration.GetValue<string>($"{serproConfigSection}:CpfUrl");

            if (!string.IsNullOrEmpty(serproBaseUrl))
            {
                data.Add("SerproConfigured", true);
                data.Add("SerproUrl", serproBaseUrl);
            }
            else
            {
                data.Add("SerproConfigured", false);
            }

            // Verificar configuração do Serasa
            var serasaConfigSection = useMockProviders
                ? "ExternalProviders:Serasa:Mock"
                : "ExternalProviders:Serasa:Real";

            var serasaBaseUrl = _configuration.GetValue<string>($"{serasaConfigSection}:BaseUrl")
                             ?? _configuration.GetValue<string>($"{serasaConfigSection}:WsdlUrl");

            if (!string.IsNullOrEmpty(serasaBaseUrl))
            {
                data.Add("SerasaConfigured", true);
                data.Add("SerasaUrl", serasaBaseUrl);
            }
            else
            {
                data.Add("SerasaConfigured", false);
            }

            // Se estiver usando mocks e temos HttpClientFactory, fazer um ping básico (opcional)
            if (useMockProviders && _httpClientFactory != null)
            {
                var warnings = new List<string>();

                // Verificar mock do Serpro (se configurado)
                if (!string.IsNullOrEmpty(serproBaseUrl) && Uri.TryCreate(serproBaseUrl, UriKind.Absolute, out var serproUri))
                {
                    try
                    {
                        using var httpClient = _httpClientFactory.CreateClient();
                        httpClient.Timeout = TimeSpan.FromSeconds(2);
                        var response = await httpClient.GetAsync(serproUri, cancellationToken);
                        data.Add("SerproMockReachable", response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Não foi possível alcançar o mock do Serpro");
                        warnings.Add("Serpro mock não está acessível");
                        data.Add("SerproMockReachable", false);
                    }
                }

                // Verificar mock do Serasa (se configurado)
                if (!string.IsNullOrEmpty(serasaBaseUrl) && Uri.TryCreate(serasaBaseUrl, UriKind.Absolute, out var serasaUri))
                {
                    try
                    {
                        using var httpClient = _httpClientFactory.CreateClient();
                        httpClient.Timeout = TimeSpan.FromSeconds(2);
                        var response = await httpClient.GetAsync(serasaUri, cancellationToken);
                        data.Add("SerasaMockReachable", response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Não foi possível alcançar o mock do Serasa");
                        warnings.Add("Serasa mock não está acessível");
                        data.Add("SerasaMockReachable", false);
                    }
                }

                // Se temos avisos, retornar Degraded ao invés de Healthy
                if (warnings.Any())
                {
                    return HealthCheckResult.Degraded(
                        $"Provedores externos configurados mas com problemas: {string.Join(", ", warnings)}",
                        data: data);
                }
            }

            return HealthCheckResult.Healthy("Provedores externos configurados corretamente", data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar health dos provedores externos");
            return HealthCheckResult.Unhealthy(
                "Erro ao verificar provedores externos",
                ex,
                new Dictionary<string, object> { { "Error", ex.Message } });
        }
    }
}
