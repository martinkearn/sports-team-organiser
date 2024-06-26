namespace STO.Wasm.Interfaces
{
	/// <summary>
	/// Service for working with Players, PlayerEntities.
	/// </summary>
	public interface IPlayerService
	{
		/// <summary>
		/// Gets all PlayerEntity objects.
		/// </summary>
		/// <returns>List of PlayerEntity.</returns>
		public List<PlayerEntity> GetPlayerEntities();

		/// <summary>
		/// Gets a list of PlayerEntity based on a list of PlayerAtGameEntity
		/// </summary>
		/// <param name="pags">List of PlayerAtGameEntity to turn into PlayerEntity</param>
		/// <returns>List of PlayerEntity.</returns>
		public List<PlayerEntity> GetPlayerEntitiesFromPags(List<PlayerAtGameEntity> pags);

		/// <summary>
		/// Gets a PlayerEntity object from RowKey
		/// </summary>
		/// <param name="rowKey">The RowKey for the PlayerEntity to match on</param>
		/// <returns>A PlayerEntity.</returns>
		public PlayerEntity GetPlayerEntity(string rowKey);
		
		/// <summary>
		/// Gets a PlayerEntity object from a UrlSegment
		/// </summary>
		/// <param name="urlSegment">The UrlSegment for the PlayerEntity to match on</param>
		/// <returns>A PlayerEntity.</returns>
		public PlayerEntity GetPlayerEntityFromUrlSegment(string urlSegment);

		/// <summary>
		/// Returns the default rate for the PlayerEntity associated with the rowKey
		/// </summary>
		/// <param name="rowKey">The Rowkey for the PlayerEntity to get the default rate for</param>
		/// <returns>A double representing the default rate.</returns>
		public double GetDefaultRateForPlayerEntity(string rowKey);
		
		/// <summary>
		/// Returns the balance of all TransactionEntities for the PlayerEntity associated with the rowKey
		/// </summary>
		/// <param name="rowKey">The Rowkey for the PlayerEntity to get the balance for</param>
		/// <returns>A double representing the balance.</returns>
		public double GetBalanceForPlayerEntity(string rowKey);

		/// <summary>
		/// Returns the Rating for the PlayerEntity associated with the rowKey
		/// </summary>
		/// <param name="rowKey">The Rowkey for the PlayerEntity to get the rating for</param>
		/// <returns>A double representing the player rating.</returns>
		public double GetRatingForPlayerEntity(string rowKey);

		/// <summary>
		/// Deletes the PlayerEntity, TransactionEntity and PlayerAtGameEntity associated with a Player.
		/// </summary>
		/// <param name="playerRowkey">The RowKey for the PlayerEntities to delete.</param>
		public Task DeletePlayerEntityAsync(string playerRowkey);

		/// <summary>
		/// Adds a new PlayerEntity.
		/// </summary>
		/// <param name="playerEntity">The PlayerEntity to upsert.</param>
		public Task UpsertPlayerEntityAsync(PlayerEntity playerEntity);
	}
}