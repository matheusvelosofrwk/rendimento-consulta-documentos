using ConsultaDocumentos.Domain.Intefaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ConsultaDocumentos.Infra.Data.Services.Cache
{
    public class InMemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<InMemoryCacheService> _logger;

        public InMemoryCacheService(
            IMemoryCache memoryCache,
            ILogger<InMemoryCacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                if (_memoryCache.TryGetValue(key, out T? cachedValue))
                {
                    _logger.LogDebug("Cache hit (in-memory) para chave: {Key}", key);
                    return Task.FromResult(cachedValue);
                }

                _logger.LogDebug("Cache miss (in-memory) para chave: {Key}", key);
                return Task.FromResult<T?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter valor do cache in-memory para chave: {Key}", key);
                return Task.FromResult<T?>(null);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expirationTime ?? TimeSpan.FromDays(90)
                };

                _memoryCache.Set(key, value, cacheOptions);
                _logger.LogDebug("Valor armazenado no cache in-memory para chave: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao armazenar valor no cache in-memory para chave: {Key}", key);
            }

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                _memoryCache.Remove(key);
                _logger.LogDebug("Valor removido do cache in-memory para chave: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover valor do cache in-memory para chave: {Key}", key);
            }

            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return Task.FromResult(_memoryCache.TryGetValue(key, out _));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência no cache in-memory para chave: {Key}", key);
                return Task.FromResult(false);
            }
        }
    }
}
