namespace STO.Models.Interfaces
{
    /// <summary>
    /// Service for working with Players and their balance.
    /// </summary>
    public interface IPlayerService
    {
        /// <summary>
        /// Converts a list of PlayerEntities to a full list of Players.
        /// </summary>
        /// <param name="playerEntities">The list of PlayerEntities to convert.</param>
        /// <returns>List of Players.</returns>
        public Task<List<Player>> GetPlayers(List<PlayerEntity> playerEntities);

        /// <summary>
        /// Converts all PlayerEntities to a full list of Players.
        /// </summary>
        /// <returns>List of Players.</returns>
        public Task<List<Player>> GetPlayers();

        /// <summary>
        /// Gets a single player by row key.
        /// </summary>
        /// <returns>A Player.</returns>
        public Task<Player> GetPlayer(string rowKey);

        /// <summary>
        /// Deletes the PlayerEntity, TransactionEntity and PlayerAtGameEntity associated with a Player.
        /// </summary>
        /// <param name="playerRowkey">The RowKey for the PlayerEntities to delete.</param>
        public Task DeletePlayer(string playerRowkey);

        /// <summary>
        /// Adds a new PlayerEntity.
        /// </summary>
        /// <param name="playerEntity">The PlayerEntity to upsert.</param>
        public Task UpsertPlayerEntity(PlayerEntity playerEntity);
    }
}