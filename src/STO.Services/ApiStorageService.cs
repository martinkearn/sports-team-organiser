using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
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
            await _tableClient.UpsertEntityAsync<T>(entity, TableUpdateMode.Replace);

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
                return _tableClient.Query<T>($"PartitionKey eq '{typeof(T).ToString()}'").ToList();
            }
        }   

        public async Task RefreshData()
        {
            await RefreshEntitiesFromStorage<PlayerEntity>();
            await RefreshEntitiesFromStorage<GameEntity>();
            await RefreshEntitiesFromStorage<TransactionEntity>();
            await RefreshEntitiesFromStorage<PlayerAtGameEntity>();
            await RefreshEntitiesFromStorage<RatingEntity>();
            _gotData = true;
        }

        private async Task RefreshEntitiesFromStorage<T>() where T : class, ITableEntity
        {
            var entities = _tableClient.Query<T>($"PartitionKey eq '{typeof(T)}'").ToList();

            var ty = typeof(T);

            if (ty == typeof(PlayerEntity))
            {
                _playerEntities = (List<PlayerEntity>)Convert.ChangeType(entities, typeof(List<PlayerEntity>));
            }

            if (ty == typeof(GameEntity))
            {
                _gameEntities = await ApiGet<GameEntity>("GameEntity");
            }

            if (ty == typeof(TransactionEntity))
            {
                _transactionEntities = (List<TransactionEntity>)Convert.ChangeType(entities, typeof(List<TransactionEntity>));
            }
                        
            if (ty == typeof(PlayerAtGameEntity))
            {
                _playerAtGameEntities = (List<PlayerAtGameEntity>)Convert.ChangeType(entities, typeof(List<PlayerAtGameEntity>));
            }

            if (ty == typeof(RatingEntity))
            {
                _ratingEntities = (List<RatingEntity>)Convert.ChangeType(entities, typeof(List<RatingEntity>));
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
    }
}