using ConsultaDocumentos.Domain.Intefaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ConsultaDocumentos.Infra.Data.Services.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(
            IDistributedCache distributedCache,
            ILogger<RedisCacheService> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);

                if (string.IsNullOrEmpty(cachedValue))
                {
                    _logger.LogDebug("Cache miss para chave: {Key}", key);
                    return null;
                }

                _logger.LogDebug("Cache hit para chave: {Key}", key);
                return JsonSerializer.Deserialize<T>(cachedValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter valor do cache Redis para chave: {Key}", key);
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value);

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expirationTime ?? TimeSpan.FromDays(90)
                };

                await _distributedCache.SetStringAsync(key, serializedValue, options, cancellationToken);
                _logger.LogDebug("Valor armazenado no cache Redis para chave: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao armazenar valor no cache Redis para chave: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _distributedCache.RemoveAsync(key, cancellationToken);
                _logger.LogDebug("Valor removido do cache Redis para chave: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover valor do cache Redis para chave: {Key}", key);
            }
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);
                return !string.IsNullOrEmpty(cachedValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência no cache Redis para chave: {Key}", key);
                return false;
            }
        }
    }
}
