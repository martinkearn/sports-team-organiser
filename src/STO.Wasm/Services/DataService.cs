using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace STO.Wasm.Services
{
    /// <inheritdoc/>
    public class DataService : IDataService
    {
        private readonly ApiConfiguration _options;

        private readonly ILocalStorageService _localStore;

        private JsonSerializerOptions _jsonSerializerOptions;

        private readonly HttpClient _httpClient;

        public DataService(IOptions<ApiConfiguration> storageConfigurationOptions, IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
        { 
            _options = storageConfigurationOptions.Value;

            _httpClient = httpClientFactory.CreateClient();

            _localStore = localStorageService;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task DeleteEntity<T>(string rowKey) where T : class, ITableEntity
        {
            // Delete entity in browser storage
            await BrowserStorageDelete<T>(rowKey);

            // Delete Entity in Api
            var apiPath = GetApiPath<T>();
            await Task.Run(() => ApiDelete(apiPath, rowKey));
        } 

        public async Task<T> UpsertEntity<T>(T entity) where T : class, ITableEntity
        {
            // Complete required values
            if (entity.RowKey == default) entity.RowKey = Guid.NewGuid().ToString();
            if (entity.PartitionKey == default) entity.PartitionKey = typeof(T).ToString();

            // Upsert entity in browser storage
            await BrowserStoragePost<T>(entity);

            // Upsert entity in Api
            var apiPath = GetApiPath<T>();
            await Task.Run(() => ApiPost<T>(apiPath, entity));
            
            // Return
            return entity;
        } 

        public async Task<List<T>> QueryEntities<T>() where T : class, ITableEntity
        {
            // Only queries from browser storage
            // TO DO need to check that cache is fresh and re-cache if not. By default, cache will load from Api when blazor app loads initially

            var data = await _localStore.GetItemAsync<List<T>>(typeof(T).Name);
            if (data is not null)
            {
                return (List<T>)Convert.ChangeType(data, typeof(List<T>));
            }
            else
            {
                return [];
            }
        }   

        public async Task LoadDataFromApi()
        {
            var playerTask = GetDataFromApi<PlayerEntity>();
            var gameTask =  GetDataFromApi<GameEntity>();
            var transactionTask =  GetDataFromApi<TransactionEntity>();
            var playerAtGameTask =  GetDataFromApi<PlayerAtGameEntity>();
            var ratingTask =  GetDataFromApi<RatingEntity>();
            var dataDetailsTask =  GetDataFromApi<DataDetailsEntity>();

            await Task.WhenAll(playerTask, gameTask, transactionTask, playerAtGameTask, ratingTask, dataDetailsTask);
        }

        private async Task GetDataFromApi<T>() where T : class, ITableEntity
        {
            var apiPath = GetApiPath<T>();
            var data = await ApiGet<T>(apiPath);
            if (data is not null)
            {
                await _localStore.SetItemAsync<List<T>>(typeof(T).Name, data);
            }
        } 

        private async Task BrowserStoragePost<T>(T entity) where T : class, ITableEntity
        {
            // Get current List of T
            var data = await _localStore.GetItemAsync<List<T>>(typeof(T).Name);

            if (data is not null)
            {
                // Remove entity from list
                data.RemoveAll(o => o.RowKey == entity.RowKey);

                // Add new entity to list
                data.Add(entity);
                
                // Save new List to browser storage
                await _localStore.SetItemAsync<List<T>>(typeof(T).Name, data);
            }
        }

        private async Task BrowserStorageDelete<T>(string rowKey) where T : class, ITableEntity
        {
            // Get current List of T
            var data = await _localStore.GetItemAsync<List<T>>(typeof(T).Name);

            if (data is not null)
            {
                // Remove entity from list
                data.RemoveAll(o => o.RowKey == rowKey);
                
                // Save new List to browser storage
                await _localStore.SetItemAsync<List<T>>(typeof(T).Name, data);
            }
        }

        private async Task<List<T>> ApiGet<T>(string path) where T : class, ITableEntity
        {
            var httpResponseMessage = await _httpClient.GetAsync($"{_options.ApiHost}/{path}");

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

        private async Task ApiPost<T>(string path, T entity) where T : class, ITableEntity   
        {
            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                $"{_options.ApiHost}/{path}", 
                entity);

            response.EnsureSuccessStatusCode();
        }

        private async Task ApiDelete(string path, string rowKey)   
        {
            using HttpResponseMessage response = await _httpClient.DeleteAsync($"{_options.ApiHost}/{path}?rowkey={rowKey}");

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