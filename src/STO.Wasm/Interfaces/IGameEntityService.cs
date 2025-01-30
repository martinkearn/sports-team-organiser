namespace STO.Wasm.Interfaces
{
	/// <summary>
	/// Service for working with GameEntity and PlayerAtGame.
	/// </summary>
	public interface IGameEntityService
    {
        /// <summary>
        /// Gets all GameEntities.
        /// </summary>
        /// <returns>List of GameEntity.</returns>
        public List<GameEntity> GetGameEntities();

        /// <summary>
        /// Gets a single GameEntity.
        /// </summary>
        /// <param name="rowKey">The RowKey for the GameEntity to get.</param>
        /// <returns>GameEntity.</returns>
        public GameEntity GetGameEntity(string rowKey);
        
        /// <summary>
        /// Gets a single GameEntity.
        /// </summary>
        /// <param name="urlSegment">The UrlSegment for the GameEntity to get.</param>
        /// <returns>GameEntity.</returns>
        public GameEntity GetGameEntityByUrlSegment(string urlSegment);

        /// <summary>
        /// Gets the next GameEntity in terms of date.
        /// </summary>
        /// <returns>GameEntity.</returns>
        public GameEntity GetNextGameEntity();

        /// <summary>
        /// Deletes the GameEntity and PlayerAtGameEntity associated with a Game.
        /// </summary>
        /// <param name="rowKey">The RowKey for the GameEntity to delete.</param>
        public Task DeleteGameEntityAsync(string rowKey);

        /// <summary>
        /// Adds a new GameEntity.
        /// </summary>
        /// <param name="gameEntity">The GameEntity to upsert.</param>
        public Task UpsertGameEntityAsync(GameEntity gameEntity);
        
		/// <summary>
		/// A string which can be used as notes when mapping a transaction to a game
		/// </summary>
		/// <param name="rowKey">The RowKey of the GameEntity to create notes for.</param>
		/// <returns>A string which can be used as notes for a Game.</returns>
		public string GetNotesForGame(string rowKey);

		/// <summary>
		/// Returns a title for the GameEntity based on the game's date, title and note
		/// </summary>
		/// <param name="rowKey">The RowKey of the GameEntity to create a title for.</param>
		/// <param name="length">Should the label be short or long.</param>
		/// <returns>A string which represents the label for the GameEntity.</returns>
		public string GetGameLabel(string rowKey, Enums.TitleLength length = Enums.TitleLength.Short);
    }
}