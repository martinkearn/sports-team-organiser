﻿namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class GameService(IDataService dataService, IRatingService ratingEntityService, IPlayerService playerService, ITransactionService transactionService) : IGameService
	{
		public async Task<List<PlayerAtGameEntity>> CalculateTeamsAsync(List<PlayerAtGameEntity> pags)
		{
			// Resolve Pags to ExpandedPags
			List<ExpandedPag> ePags = (from pag in pags 
				let ge = GetGameEntity(pag.GameRowKey) 
				let pe = playerService.GetPlayerEntity(pag.PlayerRowKey) 
				select new ExpandedPag(pag, ge, pe)).ToList();

			// Get Yes pags
			var newEPags = new List<ExpandedPag>();
            var yesPags = ePags
                .Where(o => o.PagEntity.Forecast.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(o => o.PlayerEntity.AdminRating).ToList();
            var nextTeamToGetPag = "A";

            foreach (var position in Enum.GetNames(typeof(Enums.PlayerPosition)))
            {
                // Get pags in this position
                var pagsInPosition = yesPags.Where(o => o.PlayerEntity.Position.ToString() == position.ToString());

                // Distribute pags in this position between teams
                foreach (var pagInPosition in pagsInPosition)
                {
                    // Set team for page
                    pagInPosition.PagEntity.Team = nextTeamToGetPag;
                    newEPags.Add(pagInPosition);

                    // Update pag in storage
                    await UpsertPlayerAtGameEntityAsync(pagInPosition.PagEntity);

                    // Set team for next pag
                    nextTeamToGetPag = (nextTeamToGetPag == "A") ? "B" : "A";
                }
            }

            newEPags = newEPags.OrderBy(o => o.PlayerEntity.Name).ToList();

            return newEPags.Select(ep => ep.PagEntity).ToList();
        }

		public async Task DeleteGameEntityAsync(string rowkey)
		{
			// Delete Ratings
			var ratingsForGame = ratingEntityService.GetRatingEntitiesForGame(rowkey);
			foreach (var re in ratingsForGame)
			{
				await ratingEntityService.DeleteRatingEntityAsync(re.RowKey);
			}

			// Delete PAGs
			var pagsForGame = GetPlayerAtGameEntitiesForGame(rowkey);
			foreach (var pag in pagsForGame)
			{
				await DeletePlayerAtGameEntityAsync(pag.RowKey);
			}

			// Delete game
			await dataService.DeleteEntityAsync<GameEntity>(rowkey);
		}

		public async Task DeletePlayerAtGameEntityAsync(string rowKey)
		{
			await dataService.DeleteEntityAsync<PlayerAtGameEntity>(rowKey);
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

		public string GetGameLabel(string rowKey)
		{
			if (string.IsNullOrEmpty(rowKey)) return string.Empty;
			
			var ge = GetGameEntity(rowKey);
			var gameDateLabel = ge.Date.ToString("dd MMM");
			var gameLabel = string.IsNullOrEmpty(ge.Title) ? gameDateLabel : $"{gameDateLabel} {ge.Title}";
			return gameLabel;
		}

		public List<PlayerAtGameEntity> GetPlayerAtGameEntitiesForGame(string gameRowKey)
		{
			var pagsForGame = dataService.PlayerAtGameEntities.Where(o => o.GameRowKey == gameRowKey).ToList();
			return pagsForGame;
		}

		public PlayerAtGameEntity GetPlayerAtGameEntity(string rowKey)
		{
			return dataService.PlayerAtGameEntities.First(o => o.RowKey == rowKey);
		}

		public async Task MarkAllPlayedAsync(string gameRowkey, bool played)
		{
			var pags = GetPlayerAtGameEntitiesForGame(gameRowkey);
			foreach (var pag in pags)
			{
				await TogglePlayerAtGamePlayedAsync(pag, played);
			}
		}

		public async Task TogglePlayerAtGamePlayedAsync(PlayerAtGameEntity pag, bool? played)
		{
			// Get player for pag
			var playerEntity = playerService.GetPlayerEntity(pag.PlayerRowKey);

			if (played != null)
			{
				// Set the pag value to what played is
				pag.Played = (bool)played;
			}
			else
			{
				// Just toggle the pag value
				pag.Played = !pag.Played;
			}

			// Add / remove transactions if played / not played
			if (pag.Played)
			{
				var transaction = new TransactionEntity()
				{
					PlayerRowKey = pag.PlayerRowKey,
					Amount = -playerEntity.DefaultRate,
					Date = DateTimeOffset.UtcNow,
					Notes = GetNotesForGame(pag.GameRowKey)
				};
				if (transactionService is not null)
                {
                    await transactionService.UpsertTransactionEntityAsync(transaction);
                }
			}
			else
			{
				// Get debit transactions (less than £0) for player and game
				var teForPe = transactionService.GetTransactionEntities().Where(o => o.PlayerRowKey == playerEntity.RowKey);
				var pagDebitTransactionEntities = teForPe.Where(o => o.Amount < 0);
				if (transactionService is not null)
                {
                    foreach (var pagDebitTransactionEntity in pagDebitTransactionEntities)
                    {
                        await transactionService.DeleteTransactionEntityAsync(pagDebitTransactionEntity.RowKey);
                    }
                }
			}

			// Upsert pag
			await UpsertPlayerAtGameEntityAsync(pag);
		}

		public async Task UpsertGameEntityAsync(GameEntity gameEntity)
		{
			await dataService.UpsertEntityAsync(gameEntity);
		}

		public async Task UpsertPlayerAtGameEntityAsync(PlayerAtGameEntity pagEntity)
		{
			await dataService.UpsertEntityAsync(pagEntity);
		}
	}

	public class ExpandedPag(PlayerAtGameEntity pagEntity, GameEntity gameEntity, PlayerEntity playerEntity)
	{
		public PlayerAtGameEntity PagEntity { get; } = pagEntity;

		public PlayerEntity PlayerEntity { get; } = playerEntity;

		public GameEntity GameEntity { get; set; } = gameEntity;
	}
}