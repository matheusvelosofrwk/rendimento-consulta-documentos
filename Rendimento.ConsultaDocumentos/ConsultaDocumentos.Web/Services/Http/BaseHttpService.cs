using ConsultaDocumentos.Web.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ConsultaDocumentos.Web.Services.Http
{
    public abstract class BaseHttpService
    {
        protected readonly HttpClient _httpClient;
        protected readonly JsonSerializerOptions _jsonOptions;

        protected BaseHttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        protected async Task<Result<T>> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                return await DeserializeResponse<T>(response);
            }
            catch (Exception ex)
            {
                return CreateErrorResult<T>($"Erro ao fazer requisição GET: {ex.Message}");
            }
        }

        protected async Task<Result<T>> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var content = CreateJsonContent(data);
                var response = await _httpClient.PostAsync(endpoint, content);
                return await DeserializeResponse<T>(response);
            }
            catch (Exception ex)
            {
                return CreateErrorResult<T>($"Erro ao fazer requisição POST: {ex.Message}");
            }
        }

        protected async Task<Result<T>> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                var content = CreateJsonContent(data);
                var response = await _httpClient.PutAsync(endpoint, content);
                return await DeserializeResponse<T>(response);
            }
            catch (Exception ex)
            {
                return CreateErrorResult<T>($"Erro ao fazer requisição PUT: {ex.Message}");
            }
        }

        protected async Task<Result<T>> DeleteAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                return await DeserializeResponse<T>(response);
            }
            catch (Exception ex)
            {
                return CreateErrorResult<T>($"Erro ao fazer requisição DELETE: {ex.Message}");
            }
        }

        private StringContent CreateJsonContent(object data)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return content;
        }

        private async Task<Result<T>> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            // Verificar se o conteúdo está vazio ou em branco
            if (string.IsNullOrWhiteSpace(content))
            {
                if (response.IsSuccessStatusCode)
                {
                    return CreateErrorResult<T>("Resposta vazia do servidor");
                }
                else
                {
                    return CreateErrorResult<T>($"Erro HTTP {response.StatusCode}: Resposta vazia");
                }
            }

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var result = JsonSerializer.Deserialize<Result<T>>(content, _jsonOptions);
                    return result ?? CreateErrorResult<T>("Resposta vazia do servidor");
                }
                catch (JsonException ex)
                {
                    return CreateErrorResult<T>($"Erro ao deserializar resposta: {ex.Message}. Content: {content}");
                }
            }
            else
            {
                try
                {
                    var errorResult = JsonSerializer.Deserialize<Result<T>>(content, _jsonOptions);
                    return errorResult ?? CreateErrorResult<T>($"Erro HTTP {response.StatusCode}: {content}");
                }
                catch (JsonException)
                {
                    return CreateErrorResult<T>($"Erro HTTP {response.StatusCode}: {content}");
                }
            }
        }

        private Result<T> CreateErrorResult<T>(string errorMessage)
        {
            return new Result<T>
            {
                Success = false,
                Data = default,
                Notifications = new List<string> { errorMessage }
            };
        }
    }
}
