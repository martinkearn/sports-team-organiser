using Azure.Data.Tables;

namespace STO.Api.Interfaces
{
    /// <summary>
    /// Service for working with Azure Storage.
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Adds or Updates an entity. Updates if rowkey is present, adds if not.
        /// </summary>
        /// <param name="entity">The entity of type T to upsert.</param>
        /// <returns>Stored representation of the entity of type T.</returns>
        public Task<T> UpsertEntity<T>(T entity) where T : class, ITableEntity;

        /// <summary>
        /// Deletes an entity of type T.
        /// </summary>
        /// <param name="rowKey">The rowKey value for the player to delete.</param>
        /// <returns>Nothing.</returns>
        public Task DeleteEntity<T>(string rowKey) where T : class, ITableEntity;

        /// <summary>
        /// Queries entities of type T.
        /// </summary>
        /// <returns>A list of entities of type T which match the query.</returns>
        public Task<List<T>> QueryEntities<T>() where T : class, ITableEntity;

        /// <summary>
        /// Refreshes cached data from storage.
        /// </summary>
        public Task RefreshData();
    }
}