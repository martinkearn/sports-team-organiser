namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class PlayerEntityService(ICachedDataService dataService) : IPlayerEntityService
	{
		public List<PlayerEntity> GetPlayerEntities()
		{
			return [.. dataService.PlayerEntities.OrderBy(o => o.Name)];
		}

		public PlayerEntity GetPlayerEntity(string rowKey)
		{
			var pes = dataService.PlayerEntities;
			try
			{
				return pes.First(o => o.RowKey == rowKey);
			}
			catch (Exception ex)
			{
				var m = ex.Message;
				return new PlayerEntity();
			}
		}

		public Player GetPlayer(string rowKey)
		{
			// Get PlayerEntity
			var playerEntity = GetPlayerEntity(rowKey);

			// Get Transactions
			var playersTransactions = dataService.TransactionEntities.Where(o => o.PlayerRowKey == rowKey)
				.OrderByDescending(o => o.Date)
				.ToList();

			// Get Balance
			var playerBalance = playersTransactions.Sum(o => o.Amount);

			// Construct Player
			var player = new Player(playerEntity)
			{
				Transactions = playersTransactions,
				Balance = playerBalance
			};

			return player;
		}

		public async Task DeletePlayerEntityAsync(string playerRowkey)
		{
			// Delete Ratings
			var allRatingEntities = await dataService.QueryEntitiesAsync<RatingEntity>();
			var ratingsForPlayer = allRatingEntities.Where(o => o.PlayerRowKey == playerRowkey).ToList();
			foreach (var rating in ratingsForPlayer)
			{
				await dataService.DeleteEntityAsync<RatingEntity>(rating.RowKey);
			}

			// Delete TransactionEntity
			var transactionsResult = await dataService.QueryEntitiesAsync<TransactionEntity>();
			var transactions = transactionsResult.Where(t => t.PlayerRowKey == playerRowkey);
			foreach (var transaction in transactions)
			{
				await dataService.DeleteEntityAsync<TransactionEntity>(transaction.RowKey);
			}

			// Delete PlayerAtGameEntity
			var pagsResult = await dataService.QueryEntitiesAsync<PlayerAtGameEntity>();
			var pags = pagsResult.Where(pag => pag.PlayerRowKey == playerRowkey);
			foreach (var pag in pags)
			{
				await dataService.DeleteEntityAsync<PlayerAtGameEntity>(pag.RowKey);
			}

			// Delete PlayerEntity
			await dataService.DeleteEntityAsync<PlayerEntity>(playerRowkey);
		}

		public async Task UpsertPlayerEntityAsync(PlayerEntity playerEntity)
		{
			await dataService.UpsertEntityAsync(playerEntity);
		}

	}
}