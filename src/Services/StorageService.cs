using Azure.Data.Tables;
using Microsoft.Extensions.Options;

namespace STO.Services
{
    /// <inheritdoc/>
    public class StorageService : IStorageService
    {
        private readonly StorageConfiguration _options;
        private readonly TableClient _tableClient;

        public StorageService(IOptions<StorageConfiguration> storageConfigurationOptions)
        { 
            _options = storageConfigurationOptions.Value;

            _tableClient = new TableClient(_options.ConnectionString, _options.DataTable);
            _tableClient.CreateIfNotExists();
        }

        public async Task DeleteEntity<T>(string rowKey) where T : class, ITableEntity
        {
            await _tableClient.DeleteEntityAsync(typeof(T).ToString(), rowKey);
        }

        public List<T> QueryEntities<T>(string? filter) where T : class, ITableEntity
        {
            // Switch on T to get partition key rather than passing it in? Or even use T to define partition key on creation?

            if (string.IsNullOrEmpty(filter))
            {
                filter = $"PartitionKey eq '{typeof(T).ToString()}'";
            }

            var entities = _tableClient.Query<T>(filter).ToList();

            return entities;
        }  

        public async Task<T> UpsertEntity<T>(T entity) where T : class, ITableEntity
        {
            // Complete required values
            if (entity.RowKey == default) entity.RowKey = Guid.NewGuid().ToString();
            if (entity.PartitionKey == default) entity.PartitionKey = typeof(T).ToString();

            // Upsert entity
            await _tableClient.UpsertEntityAsync<T>(entity, TableUpdateMode.Replace);

            // Get player from storage
            var upsertedEntity = this.QueryEntities<T>($"RowKey eq '{entity.RowKey}'").First();

            // Return
            return upsertedEntity;
        }        
    }
}