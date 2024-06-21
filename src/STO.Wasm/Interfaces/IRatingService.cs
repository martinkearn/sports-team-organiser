namespace STO.Wasm.Interfaces
{
    /// <summary>
    /// Service for working with Ratings.
    /// </summary>
    public interface IRatingService
    {
        /// <summary>
        /// Gets all RatingEntities.
        /// </summary>
        /// <returns>List of RatingEntity.</returns>
        public List<RatingEntity> GetRatingEntities();

        /// <summary>
        /// Gets a single RatingEntity by row key.
        /// </summary>
        /// <param name="rowKey">The RowKey for the RatingEntity to get.</param>
        /// <returns>A RatingEntity.</returns>
        public RatingEntity GetRatingEntity(string rowKey);
        
        /// <summary>
        /// Gets a single RatingEntity by UrlSegment.
        /// </summary>
        /// <param name="urlSegment">The UrlSegment for the RatingEntity to get.</param>
        /// <returns>A RatingEntity.</returns>
        public RatingEntity GetRatingEntityByUrlSegment(string urlSegment);

		/// <summary>
		/// Gets all RatingEntities for a PlayerEntity.
		/// </summary>
		/// <param name="playerRowKey">The RowKey for the PlayerEntity to get RatingEntities for.</param>
		/// <returns>List of RatingEntity for the given PlayerEntity.</returns>
		public List<RatingEntity> GetRatingEntitiesForPlayer(string playerRowKey);

		/// <summary>
		/// Gets all RatingEntities for a GameEntity.
		/// </summary>
		/// <param name="gameRowKey">The RowKey for the GameEntity to get RatingEntities for.</param>
		/// <returns>List of RatingEntity for the given GameEntity.</returns>
		public List<RatingEntity> GetRatingEntitiesForGame(string gameRowKey);

        /// <summary>
        /// Deletes a RatingEntity.
        /// </summary>
        /// <param name="rowKey">The RowKey for the RatingEntity to delete.</param>
        public Task DeleteRatingEntityAsync(string rowKey);

        /// <summary>
        /// Adds a new RatingEntity.
        /// </summary>
        /// <param name="ratingEntity">The RatingEntity to upsert.</param>
        public Task UpsertRatingEntityAsync(RatingEntity ratingEntity);

		/// <summary>
		/// Formats RatingEntity time in a consistent way.
		/// </summary>
		/// <param name="rowKey">The RowKey for the RatingEntity to format the time for</param>
		/// <returns>A formated time string.</returns>
		public string FormatRatingTime(string rowKey);
    }
}