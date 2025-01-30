using STO.Models.Interfaces;

namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class GameEntityService(IDataService dataService, IRatingEntityService ratingEntityEntityService, ITransactionService transactionService) : IGameEntityService
	{
		public async Task DeleteGameEntityAsync(string rowkey)
		{
			// Delete Ratings
			var ratingsForGame = ratingEntityEntityService.GetRatingEntitiesForGame(rowkey);
			foreach (var re in ratingsForGame)
			{
				await ratingEntityEntityService.DeleteRatingEntityAsync(re.RowKey);
			}

			// Delete PAGs
			//TO DO: get this directly form data service
			var pagsForGame = GetPlayerAtGameEntitiesForGame(rowkey);
			foreach (var pag in pagsForGame)
			{
				//TO DO: Do this directly via data service
				await DeletePlayerAtGameEntityAsync(pag.RowKey);
			}

			// Delete game
			await dataService.DeleteEntityAsync<GameEntity>(rowkey);
		}
		
		public List<GameEntity> GetGameEntities()
		{
			return [.. dataService.GameEntities.OrderByDescending(o => o.Date)];
		}

		public GameEntity GetGameEntity(string rowKey)
		{
			var ges = GetGameEntities();
			return ges.First(o => o.RowKey == rowKey);
		}

		public GameEntity GetGameEntityByUrlSegment(string urlSegment)
		{
			var ges = GetGameEntities();
			return ges.First(o => o.UrlSegment == urlSegment);
		}

		public GameEntity GetNextGameEntity()
		{
			var ges = GetGameEntities();
			var gesOrderByDescending = ges.OrderByDescending(o => o.Date.DateTime);
			return gesOrderByDescending.First();
		}

		public string GetNotesForGame(string rowKey)
		{
			var ge = GetGameEntity(rowKey);
			if (ge is null) return string.Empty;
			var notes = $"For game {ge.Date.Date:dd MMM yyyy}";
			return notes;
		}

		public string GetGameLabel(string rowKey, Enums.TitleLength length)
		{
			if (string.IsNullOrEmpty(rowKey)) return string.Empty;
			
			var ge = GetGameEntity(rowKey);
			var gameDateLabel = length switch
			{
				Enums.TitleLength.Short => ge.Date.ToString("dd MMM"),
				Enums.TitleLength.Long => ge.Date.DateTime.ToLongDateString(),
				_ => throw new ArgumentOutOfRangeException(nameof(length), length, null)
			};

			var gameLabel = string.IsNullOrEmpty(ge.Title) ? gameDateLabel : $"{gameDateLabel} {ge.Title}";
			return gameLabel;
		}
		
		public async Task UpsertGameEntityAsync(GameEntity gameEntity)
		{
			await dataService.UpsertEntityAsync(gameEntity);
		}
	}
}
