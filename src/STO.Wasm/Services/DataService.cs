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
            // Delete Entity
            var apiPath = GetApiPath<T>();
            await ApiDelete(apiPath, rowKey);

            // Refresh data from storage
            await RefreshEntitiesFromStorage<T>();
        } 

        public async Task<T> UpsertEntity<T>(T entity) where T : class, ITableEntity
        {
            // Complete required values
            if (entity.RowKey == default) entity.RowKey = Guid.NewGuid().ToString();
            if (entity.PartitionKey == default) entity.PartitionKey = typeof(T).ToString();

            // Upsert entity
            var apiPath = GetApiPath<T>();
            await ApiPost<T>(apiPath, entity);

            // Refresh data from storage
            await RefreshEntitiesFromStorage<T>();
            
            // Return
            return entity;
        } 

        public async Task<List<T>> QueryEntities<T>() where T : class, ITableEntity
        {
            await EnsureBrowserData();

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

        public async Task RefreshData()
        {
            // Refresh caches
            var playerTask = RefreshEntitiesFromStorage<PlayerEntity>();
            var gameTask =  RefreshEntitiesFromStorage<GameEntity>();
            var transactionTask =  RefreshEntitiesFromStorage<TransactionEntity>();
            var playerAtGameTask =  RefreshEntitiesFromStorage<PlayerAtGameEntity>();
            var ratingTask =  RefreshEntitiesFromStorage<RatingEntity>();
            var dataDetailsTask =  RefreshEntitiesFromStorage<DataDetailsEntity>();

            await Task.WhenAll(playerTask, gameTask, transactionTask, playerAtGameTask, ratingTask, dataDetailsTask);
        }

        private async Task RefreshEntitiesFromStorage<T>() where T : class, ITableEntity
        {
            var apiPath = GetApiPath<T>();
            var data = await ApiGet<T>(apiPath);
            if (data is not null)
            {
                if (typeof(T) == typeof(DataDetailsEntity))
                {
                    await _localStore.SetItemAsync<T>(typeof(T).Name, data.FirstOrDefault());
                }
                else
                {
                    await _localStore.SetItemAsync<List<T>>(typeof(T).Name, data);
                }
            }
        } 

        private async Task EnsureBrowserData()
        {
/*             // TO DO - need something here to ensure we've got the latest data from the actual API
            var gotData = await _localStore.GetItemAsStringAsync(DataRefreshedKey); 
            if (gotData is null)
            { */
                await RefreshData();
                //await _localStore.SetItemAsync(DataRefreshedKey, $"{DateTimeOffset.Now.ToUnixTimeSeconds()}");
            //}
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

        private async Task ApiPut(string path)   
        {
            using HttpResponseMessage response = await _httpClient.PutAsync($"{_options.ApiHost}/{path}", null);

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