using Azure.Data.Tables;
using Microsoft.Extensions.Options;

namespace STO.Api.Services
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
            // Delete Entity
            await _tableClient.DeleteEntityAsync(typeof(T).ToString(), rowKey);

            // Update DataDetails
            await UpdateDataDetails();
        } 

        public async Task<T> UpsertEntity<T>(T entity) where T : class, ITableEntity
        {
            // Complete required values
            if (entity.RowKey == default) entity.RowKey = Guid.NewGuid().ToString();
            if (entity.PartitionKey == default) entity.PartitionKey = typeof(T).ToString();

            // Upsert entity
            await _tableClient.UpsertEntityAsync<T>(entity, TableUpdateMode.Replace);

            // Update DataDetails
            await UpdateDataDetails();
            
            // Return
            return entity;
        } 

        public async Task<List<T>> QueryEntities<T>() where T : class, ITableEntity
        {
            var entitiesPages = _tableClient.QueryAsync<T>(x => x.PartitionKey == $"{typeof(T)}", maxPerPage: 1000);
            var entities = new List<T>();
            await foreach (var entity in entitiesPages)
            {
                entities.Add(entity);
            }
            return entities;
        }   

        private async Task UpdateDataDetails()
        {
            // Create DataDetailsEntity
            var dataDetailsEntity = new DataDetailsEntity();

            // Upsert entity
            await _tableClient.UpsertEntityAsync<DataDetailsEntity>(dataDetailsEntity, TableUpdateMode.Replace);
        }    
    }
}