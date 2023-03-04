using Azure.Data.Tables;

namespace STO.Interfaces
{
    /// <summary>
    /// Service for working with Azure Storage.
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Adds or Updates an entity. Updates if rowkey is present, adds if not.
        /// </summary>
        /// <param name="partitionKey">The PartitionKey to identify the entity with.</param>
        /// <param name="entity">The entity of tyep T to upsert.</param>
        /// <returns>Stored representation of the entity of type T.</returns>
        public Task<T> UpsertEntity<T>(string partitionKey, T entity) where T : class, ITableEntity;

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="partitionKey">The PartitionKey to identify the entity with.</param>
        /// <param name="rowKey">The rowKey value for the player to delete.</param>
        /// <returns>Nothing.</returns>
        public Task DeleteEntity(string partitionKey, string rowKey);

        /// <summary>
        /// Queries entities of type T using an Odata query syntax.
        /// </summary>
        /// <param name="partitionKey">The PartitionKey to identify the entities to query on.</param>
        /// <param name="player">The OData filter string.</param>
        /// <returns>A list of entities of type T which match the query.</returns>
        public List<T> QueryEntities<T>(string partitionKey, string? filter) where T : class, ITableEntity;

        /// <summary>
        /// Returns configuration data.
        /// </summary>
        /// <returns>StorageConfiguration based on current environment.</returns>
        public StorageConfiguration GetConfiguration();
    }
}