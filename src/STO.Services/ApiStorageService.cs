using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using STO.Models;
using STO.Models.Interfaces;

namespace STO.Services
{
    /// <inheritdoc/>
    public class ApiStorageService : IStorageService
    {
        private readonly StorageConfiguration _options;
        private List<PlayerEntity> _playerEntities;
        private List<GameEntity> _gameEntities;
        private List<TransactionEntity> _transactionEntities;
        private List<PlayerAtGameEntity> _playerAtGameEntities;
        private List<RatingEntity> _ratingEntities;
        private readonly TableClient _tableClient;

        private bool _gotData;

        private JsonSerializerOptions _jsonSerializerOptions;

        private readonly HttpClient _httpClient;

        public ApiStorageService(IOptions<StorageConfiguration> storageConfigurationOptions, IHttpClientFactory httpClientFactory)
        { 
            _options = storageConfigurationOptions.Value;

            _tableClient = new TableClient(_options.ConnectionString, _options.DataTable);
            _tableClient.CreateIfNotExists();

            _httpClient = httpClientFactory.CreateClient();

            _gotData = false;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task DeleteEntity<T>(string rowKey) where T : class, ITableEntity
        {
            if(!_gotData) await RefreshData();

            await _tableClient.DeleteEntityAsync(typeof(T).ToString(), rowKey);

            // Refresh data from storage
            await RefreshEntitiesFromStorage<T>();
        } 

        public async Task<T> UpsertEntity<T>(T entity) where T : class, ITableEntity
        {
            if(!_gotData) await RefreshData();

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
            if(!_gotData) await RefreshData();

            var ty = typeof(T);
            if (ty == typeof(PlayerEntity))
            {
                return (List<T>)Convert.ChangeType(_playerEntities, typeof(List<T>));
            }
            else if (ty == typeof(GameEntity))
            {
                return (List<T>)Convert.ChangeType(_gameEntities, typeof(List<T>));
            }
            else if (ty == typeof(TransactionEntity))
            {
                return (List<T>)Convert.ChangeType(_transactionEntities, typeof(List<T>));
            }
            else if (ty == typeof(PlayerAtGameEntity))
            {
                return (List<T>)Convert.ChangeType(_playerAtGameEntities, typeof(List<T>));
            }
            else if (ty == typeof(RatingEntity))
            {
                return (List<T>)Convert.ChangeType(_ratingEntities, typeof(List<T>));
            }
            else
            {
                return null;
            }
        }   

        public async Task RefreshData()
        {
            var playerTask = RefreshEntitiesFromStorage<PlayerEntity>();
            var gameTask =  RefreshEntitiesFromStorage<GameEntity>();
            var transactionTask =  RefreshEntitiesFromStorage<TransactionEntity>();
            var playerAtGameTask =  RefreshEntitiesFromStorage<PlayerAtGameEntity>();
            var ratingTask =  RefreshEntitiesFromStorage<RatingEntity>();

            await Task.WhenAll(playerTask, gameTask, transactionTask, playerAtGameTask, ratingTask);
            
            _gotData = true;
        }

        private async Task RefreshEntitiesFromStorage<T>() where T : class, ITableEntity
        {
            var ty = typeof(T);
            var apiPath = GetApiPath<T>();

            if (ty == typeof(PlayerEntity))
            {
                _playerEntities = await ApiGet<PlayerEntity>(apiPath);
            }

            if (ty == typeof(GameEntity))
            {
                _gameEntities = await ApiGet<GameEntity>(apiPath);
            }

            if (ty == typeof(TransactionEntity))
            {
                _transactionEntities = await ApiGet<TransactionEntity>(apiPath);
            }
                        
            if (ty == typeof(PlayerAtGameEntity))
            {
                _playerAtGameEntities = await ApiGet<PlayerAtGameEntity>(apiPath);
            }

            if (ty == typeof(RatingEntity))
            {
                _ratingEntities = await ApiGet<RatingEntity>(apiPath);
            }
        } 

        private async Task<List<T>> ApiGet<T>(string path) where T : class, ITableEntity
        {
            var httpResponseMessage = await _httpClient.GetAsync($"{_options.ApiHost}/{path}");

            List<T> response = new();

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var content = await httpResponseMessage.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<IEnumerable<T>>(content, _jsonSerializerOptions);
                response = result.ToList();
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

        private string GetApiPath<T>()
        {
            var ty = typeof(T);
            var apiPath = ty.ToString().Replace("STO.Models.", String.Empty);
            return apiPath;
        }
    }
}