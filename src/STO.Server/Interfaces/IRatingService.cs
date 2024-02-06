namespace STO.Server.Interfaces
{
    /// <summary>
    /// Service for working with Ratings.
    /// </summary>
    public interface IRatingService
    {
        /// <summary>
        /// Converts all RatingEntities to a full list of Ratings.
        /// </summary>
        /// <returns>List of Ratings.</returns>
        public Task<List<Rating>> GetRatings();

        /// <summary>
        /// Gets a single Rating by row key.
        /// </summary>
        /// <param name="rowKey">The RowKey for the RatingEntity to get.</param>
        /// <returns>A Rating.</returns>
        public Task<Rating> GetRating(string rowKey);

        /// <summary>
        /// Gets all Ratings for a player.
        /// </summary>
        /// <param name="playerRowKey">The RowKey for the PlayerEntity to get ratings for.</param>
        /// <returns>List of Ratings for the given Player.</returns>
        public Task<List<Rating>> GetRatingsForPlayer(string playerRowKey);

        /// <summary>
        /// Deletes a RatingEntity.
        /// </summary>
        /// <param name="rowKey">The RowKey for the RatingEntity to delete.</param>
        public Task DeleteRatingEntity(string rowKey);

        /// <summary>
        /// Adds a new RatingEntity.
        /// </summary>
        /// <param name="ratingEntity">The RatingEntity to upsert.</param>
        public Task UpsertRatingEntity(RatingEntity playerEntity);
    }
}