using Azure.Data.Tables;
using Microsoft.Extensions.Options;

namespace STO.Services
{
    /// <inheritdoc/>
    public class StorageService : IStorageService
    {
        private readonly StorageConfiguration _options;
        private readonly TableClient _tableClient;

        private List<PlayerEntity> _playerEntities;

        public StorageService(IOptions<StorageConfiguration> storageConfigurationOptions)
        { 
            _options = storageConfigurationOptions.Value;

            _tableClient = new TableClient(_options.ConnectionString, _options.DataTable);
            _tableClient.CreateIfNotExists();

            // Load initial data
            RefreshEntitiesFromStorage<PlayerEntity>();
        }

        public async Task DeleteEntity<T>(string rowKey) where T : class, ITableEntity
        {
            await _tableClient.DeleteEntityAsync(typeof(T).ToString(), rowKey);

            // Refresh data from storage
            RefreshEntitiesFromStorage<T>();
        }

        public List<T> QueryEntities<T>(string filter) where T : class, ITableEntity
        {
            if (string.IsNullOrEmpty(filter))
            {
                filter = $"PartitionKey eq '{typeof(T).ToString()}'";
            }
            else
            {
                filter += $"and PartitionKey eq '{typeof(T).ToString()}'";
            }


            if (typeof(T) == typeof(PlayerEntity))
            {
                var nl = (List<T>)Convert.ChangeType(_playerEntities, typeof(List<T>));
                return nl;
            }
            else
            {
                return _tableClient.Query<T>(filter).ToList();
            }
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

        private void RefreshEntitiesFromStorage<T>() where T : class, ITableEntity
        {
            var entities = _tableClient.Query<T>($"PartitionKey eq '{typeof(T).ToString()}'").ToList();
            if (typeof(T) == typeof(PlayerEntity))
            {
                _playerEntities = (List<PlayerEntity>)Convert.ChangeType(entities, typeof(List<PlayerEntity>));
            }
        }     
    }
}