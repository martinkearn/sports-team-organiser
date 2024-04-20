namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class PlayerEntityService(ICachedDataService dataService, ITransactionEntityService transactionEntityService) : IPlayerEntityService
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

		public double GetDefaultRateForPlayerEntity(string rowKey)
		{
			var pe = GetPlayerEntity(rowKey);
			return pe.DefaultRate;
		}

		public double GetBalanceForPlayerEntity(string rowKey)
		{
			var transactions = transactionEntityService.GetTransactionEntitiesForPlayerEntity(rowKey);
			return transactions.Sum(o => o.Amount);
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