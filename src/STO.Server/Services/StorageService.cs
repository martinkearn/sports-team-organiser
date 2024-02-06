using Azure.Data.Tables;
using Microsoft.Extensions.Options;

namespace STO.Server.Services
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

        public StorageService(IOptions<StorageConfiguration> storageConfigurationOptions)
        { 
            _options = storageConfigurationOptions.Value;

            _tableClient = new TableClient(_options.ConnectionString, _options.DataTable);
            _tableClient.CreateIfNotExists();

            // Load initial data
            RefreshData();
        }

        public async Task DeleteEntity<T>(string rowKey) where T : class, ITableEntity
        {
            await _tableClient.DeleteEntityAsync(typeof(T).ToString(), rowKey);

            // Refresh data from storage
            RefreshEntitiesFromStorage<T>();
        } 

        public async Task<T> UpsertEntity<T>(T entity) where T : class, ITableEntity
        {
            // Complete required values
            if (entity.RowKey == default) entity.RowKey = Guid.NewGuid().ToString();
            if (entity.PartitionKey == default) entity.PartitionKey = typeof(T).ToString();

            // Upsert entity
            await _tableClient.UpsertEntityAsync<T>(entity, TableUpdateMode.Replace);

            // Refresh data from storage
            RefreshEntitiesFromStorage<T>();
            
            // Return
            return entity;
        } 

        public List<T> QueryEntities<T>() where T : class, ITableEntity
        {
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
            else
            {
                return _tableClient.Query<T>($"PartitionKey eq '{typeof(T).ToString()}'").ToList();
            }
        }   

        public void RefreshData()
        {
            RefreshEntitiesFromStorage<PlayerEntity>();
            RefreshEntitiesFromStorage<GameEntity>();
            RefreshEntitiesFromStorage<TransactionEntity>();
            RefreshEntitiesFromStorage<PlayerAtGameEntity>();
        }

        private void RefreshEntitiesFromStorage<T>() where T : class, ITableEntity
        {
            var entities = _tableClient.Query<T>($"PartitionKey eq '{typeof(T).ToString()}'").ToList();

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
        }     
    }
}