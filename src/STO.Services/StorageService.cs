using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using STO.Models;
using STO.Models.Interfaces;

namespace STO.Services
{
    /// <inheritdoc/>
    public class StorageService : IStorageService
    {
        private readonly StorageConfiguration _options;
        private readonly TableClient _tableClient;
        private List<PlayerEntity> _playerEntities;
        private List<GameEntity> _gameEntities;
        private List<TransactionEntity> _transactionEntities;
        private List<PlayerAtGameEntity> _playerAtGameEntities;
        private List<RatingEntity> _ratingEntities;
        private bool _doneInitialLoad;

        public StorageService(IOptions<StorageConfiguration> storageConfigurationOptions)
        { 
            _options = storageConfigurationOptions.Value;

            _tableClient = new TableClient(_options.ConnectionString, _options.DataTable);
            _tableClient.CreateIfNotExists();
        }

        public async Task DeleteEntity<T>(string rowKey) where T : class, ITableEntity
        {
            //Get data if we have not done so yet
            if (!_doneInitialLoad) await RefreshData();

            await _tableClient.DeleteEntityAsync(typeof(T).ToString(), rowKey);

            // Refresh data from storage
            await RefreshEntitiesFromStorage<T>();
        } 

        public async Task<T> UpsertEntity<T>(T entity) where T : class, ITableEntity
        {
            //Get data if we have not done so yet
            if (!_doneInitialLoad) await RefreshData();

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
            //Get data if we have not done so yet
            if (!_doneInitialLoad) await RefreshData();

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
                var entitiesPages = _tableClient.QueryAsync<T>(x => x.PartitionKey == $"{typeof(T)}", maxPerPage: 1000);
                var entities = new List<T>();
                await foreach (var entity in entitiesPages)
                {
                    entities.Add(entity);
                }
                return entities;
            }
        }   

        public async Task RefreshData()
        {
            await RefreshEntitiesFromStorage<PlayerEntity>();
            await RefreshEntitiesFromStorage<GameEntity>();
            await RefreshEntitiesFromStorage<TransactionEntity>();
            await RefreshEntitiesFromStorage<PlayerAtGameEntity>();
            await RefreshEntitiesFromStorage<RatingEntity>();
            _doneInitialLoad = true;
        }

        private async Task RefreshEntitiesFromStorage<T>() where T : class, ITableEntity
        {
            var entitiesPages = _tableClient.QueryAsync<T>(x => x.PartitionKey == $"{typeof(T)}", maxPerPage: 1000);
            var entities = new List<T>();
            await foreach (var entity in entitiesPages)
            {
                entities.Add(entity);
            }

            var ty = typeof(T);

            if (ty == typeof(PlayerEntity))
            {
                _playerEntities = (List<PlayerEntity>)Convert.ChangeType(entities, typeof(List<PlayerEntity>));
            }

            if (ty == typeof(GameEntity))
            {
                _gameEntities = (List<GameEntity>)Convert.ChangeType(entities, typeof(List<GameEntity>));
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
    }
}