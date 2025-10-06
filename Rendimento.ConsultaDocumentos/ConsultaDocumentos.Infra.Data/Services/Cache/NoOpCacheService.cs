using ConsultaDocumentos.Domain.Intefaces;
using Microsoft.Extensions.Logging;

namespace ConsultaDocumentos.Infra.Data.Services.Cache
{
    /// <summary>
    /// Implementação de cache que não faz nada (No Operation).
    /// Usado quando o Redis está desabilitado, forçando consultas diretas aos provedores.
    /// </summary>
    public class NoOpCacheService : ICacheService
    {
        private readonly ILogger<NoOpCacheService> _logger;

        public NoOpCacheService(ILogger<NoOpCacheService> logger)
        {
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            _logger.LogDebug("Cache desabilitado - GetAsync retornando null para chave: {Key}", key);
            return Task.FromResult<T?>(null);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default) where T : class
        {
            _logger.LogDebug("Cache desabilitado - SetAsync ignorado para chave: {Key}", key);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Cache desabilitado - RemoveAsync ignorado para chave: {Key}", key);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Cache desabilitado - ExistsAsync retornando false para chave: {Key}", key);
            return Task.FromResult(false);
        }
    }
}
