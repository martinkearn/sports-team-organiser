using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace STO.Wasm.Services
{
    /// <inheritdoc/>
    public class ApiService : IApiService
    {
        private readonly ApiConfiguration _options;

        private JsonSerializerOptions _jsonSerializerOptions;

        private readonly HttpClient _httpClient;

        public ApiService(IOptions<ApiConfiguration> storageConfigurationOptions, IHttpClientFactory httpClientFactory)
        { 
            _options = storageConfigurationOptions.Value;

            _httpClient = httpClientFactory.CreateClient();

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<List<T>> ApiGetAsync<T>() where T : class, ITableEntity
        {
            var apiPath = GetApiPath<T>();
            var httpResponseMessage = await _httpClient.GetAsync($"{_options.ApiHost}/{apiPath}");

            List<T> response = [];

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var content = await httpResponseMessage.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<IEnumerable<T>>(content, _jsonSerializerOptions);
                if (result is not null)
                {
                    return result.ToList();
                }
            }

            return response;
        } 

        public async Task ApiPostAsync<T>(T entity) where T : class, ITableEntity   
        {
            var apiPath = GetApiPath<T>();
            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                $"{_options.ApiHost}/{apiPath}", 
                entity);

            response.EnsureSuccessStatusCode();
        }

        public async Task ApiDeleteAsync<T>(string rowKey) where T : class, ITableEntity   
        {
            var apiPath = GetApiPath<T>();
            using HttpResponseMessage response = await _httpClient.DeleteAsync($"{_options.ApiHost}/{apiPath}?rowkey={rowKey}");

            response.EnsureSuccessStatusCode();
        }

        private static string GetApiPath<T>()
        {
            var ty = typeof(T);
            var apiPath = ty.ToString().Replace("STO.Models.", string.Empty);
            return apiPath;
        }
    }
}