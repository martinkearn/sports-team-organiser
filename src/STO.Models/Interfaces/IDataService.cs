using Azure.Data.Tables;

namespace STO.Models.Interfaces
{
    /// <summary>
    /// Service for working with Azure Storage.
    /// </summary>
    public interface IDataService
    {
        public List<PlayerEntity> PlayerEntities { get; set; }
		public List<TransactionEntity> TransactionEntities { get; set; }
		public List<RatingEntity> RatingEntities { get; set; }
		public List<GameEntity> GameEntities { get; set; }
		public List<PlayerAtGameEntity> PlayerAtGameEntities { get; set; }

		/// <summary>
		/// Adds or Updates an entity. Updates if rowkey is present, adds if not.
		/// </summary>
		/// <param name="entity">The entity of type T to upsert.</param>
		/// <returns>Stored representation of the entity of type T.</returns>
		public Task<T> UpsertEntityAsync<T>(T entity) where T : class, ITableEntity;

        /// <summary>
        /// Deletes an entity of type T.
        /// </summary>
        /// <param name="rowKey">The rowKey value for the player to delete.</param>
        /// <returns>Nothing.</returns>
        public Task DeleteEntityAsync<T>(string rowKey) where T : class, ITableEntity;

        /// <summary>
        /// Queries entities of type T.
        /// </summary>
        /// <returns>A list of entities of type T which match the query.</returns>
        public Task<List<T>> QueryEntitiesAsync<T>() where T : class, ITableEntity;

		/// <summary>
		/// Loads/Reloads the raw data from the API
		/// </summary>
		/// <param name="forceApi">Forces a data refresh from Api even if local data is up to date.</param>
		/// <param name="forceLocalOnly">Only loads data from local storage without any Api or Data Details Entity checks.</param>
		public Task LoadDataAsync(bool forceApi, bool forceLocalOnly);

		/// <summary>
		/// Clear the local data cache
		/// </summary>
		public Task ClearLocalDataAsync();
    }
}