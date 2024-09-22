using System.Globalization;

namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class PlayerService(IDataService dataService, ITransactionService transactionService) : IPlayerService
	{
		public List<PlayerEntity> GetPlayerEntities()
		{
			return [.. dataService.PlayerEntities.OrderBy(o => o.Name)];
		}

		public List<PlayerEntity> GetPlayerEntitiesFromPags(List<PlayerAtGameEntity> pags)
		{
			var allPes = GetPlayerEntities();
			var pes = pags.Select(pag => allPes.First(o => o.RowKey == pag.PlayerRowKey)).ToList();
			return pes;
		}

		public List<(PlayerEntity, int)> GetRecentPlayerEntities(DateTime dateRangeStart, DateTime dateRangeEnd)
		{
			// Get all players
			var allPlayers = GetPlayerEntities();
			
			// Get all games within range
			var recentGames = dataService.GameEntities
				.Where(g => g.Date.DateTime > dateRangeStart)
				.Where(g => g.Date.DateTime < dateRangeEnd)
				.ToList();
			
			// Get all players at recent games
			var recentPags = dataService.PlayerAtGameEntities
				.Where(pag => recentGames.Select(g => g.RowKey).Contains(pag.GameRowKey))
				.ToList();
			
			// Get PlayerEntity with count
			List<(PlayerEntity PlayerEntity, int GameCount)> recentPlayers = [];
			foreach (var recentPag in recentPags)
			{
				if (recentPlayers.Any(p => p.PlayerEntity.RowKey == recentPag.PlayerRowKey)) continue;
				{
					var player = allPlayers.First(p => p.RowKey == recentPag.PlayerRowKey);
					var gameCount = recentPags.Count(pag => pag.PlayerRowKey == recentPag.PlayerRowKey);
					var t = (PlayerEntity:player, GameCount:gameCount);
					recentPlayers.Add(t);
				}
			}

			return recentPlayers.OrderByDescending(tuple => tuple.GameCount).ToList();
		}

		public PlayerEntity GetPlayerEntity(string rowKey)
		{
			var pes = dataService.PlayerEntities;
			return pes.First(o => o.RowKey == rowKey);
		}

		public PlayerEntity GetPlayerEntityFromUrlSegment(string urlSegment)
		{
			var pes = dataService.PlayerEntities;
			return pes.First(o => o.UrlSegment.ToLowerInvariant().Equals(urlSegment, StringComparison.InvariantCultureIgnoreCase));
		}

		public double GetDefaultRateForPlayerEntity(string rowKey)
		{
			var pe = GetPlayerEntity(rowKey);
			return pe.DefaultRate;
		}

		public double GetBalanceForPlayerEntity(string rowKey)
		{
			var transactions = transactionService.GetTransactionEntitiesForPlayerEntity(rowKey);
			return transactions.Sum(o => o.Amount);
		}

		public double GetRatingForPlayerEntity(string rowKey)
		{
			// TO DO: Eventually this will get an average rating based on RatingEntity when we have the data to support that
			var pe = GetPlayerEntity(rowKey);
			return pe.AdminRating;
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
			//playerEntity.UrlSegment = playerEntity.Name.Replace(" ", "-").ToLowerInvariant();
			await dataService.UpsertEntityAsync(playerEntity);
		}
		
	}
}