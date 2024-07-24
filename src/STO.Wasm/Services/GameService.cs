namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class GameService(IDataService dataService, IRatingService ratingEntityService, IPlayerService playerService, ITransactionService transactionService) : IGameService
	{
		public async Task<List<PlayerAtGameEntity>> CalculateTeamsAsync(List<PlayerAtGameEntity> pags)
		{
			// Resolve Pags to ExpandedPags
			var ePags = ExpandPags(pags);

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

		public async Task ResetTeamsAsync(string gameRowKey)
		{
			var pags = GetPlayerAtGameEntitiesForGame(gameRowKey);
			
			// Using a For loop because we are modifying the collection as we go
			var index = 0;
			for (; index < pags.Count; index++)
			{
				var pag = pags[index];
				pag.Team = string.Empty;
				await UpsertPlayerAtGameEntityAsync(pag);
			}
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

		public string GetPlayerAtGameLabel(string rowKey, Enums.TitleLength length = Enums.TitleLength.Short)
		{
			// Get expanded PAG
			var pag = GetPlayerAtGameEntity(rowKey);
			var pagInList = new List<PlayerAtGameEntity> { pag };
			var expandedPags = ExpandPags(pagInList);
			var expandedPag = expandedPags.First();
			
			// Construct label
			var gameLabel = GetGameLabel(pag.GameRowKey, Enums.TitleLength.Short);
			var pagLabel = length switch
			{
				Enums.TitleLength.Short => expandedPag.PlayerEntity.Name,
				Enums.TitleLength.Long => $"{expandedPag.PlayerEntity.Name} at {gameLabel}",
				_ => throw new ArgumentOutOfRangeException(nameof(length), length, null)
			};
			
			return pagLabel;
		}

		public PlayerAtGameEntity? GetMostRecentPlayerAtGameForGame(string rowKey)
		{
			var pags = GetPlayerAtGameEntitiesForGame(rowKey);
			var orderedPags = pags.OrderByDescending(o => o.Timestamp);
			return orderedPags.FirstOrDefault();
		}

		public List<PlayerAtGameEntity> GetPlayerAtGameEntitiesForGame(string gameRowKey)
		{
			var pagsForGame = dataService.PlayerAtGameEntities.Where(o => o.GameRowKey == gameRowKey).ToList();
			
			// Resolve Pags to ExpandedPags
			var ePags = ExpandPags(pagsForGame);
			
			// Sort by position
			ePags = ePags.OrderBy(o => o.PlayerEntity.Position).ToList();
			
			// Re-create list of pags
			var orderedPags = ePags.Select(epag => epag.PagEntity).ToList();

			return orderedPags;
		}

		public PlayerAtGameEntity GetPlayerAtGameEntity(string rowKey)
		{
			return dataService.PlayerAtGameEntities.First(o => o.RowKey == rowKey);
		}

		public PlayerAtGameEntity GetPlayerAtGameEntityByUrlSegment(string urlSegment)
		{
			return dataService.PlayerAtGameEntities.First(o => o.UrlSegment == urlSegment);
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

                await transactionService.UpsertTransactionEntityAsync(transaction);
			}
			else
			{
				// Get debit transactions (less than £0) for player and game
				var teForPe = transactionService.GetTransactionEntities()
					.Where(o => o.PlayerRowKey == playerEntity.RowKey)
					.OrderByDescending(o => o.Date);
				var firstPagDebitTransactionEntities = teForPe.FirstOrDefault(o => o.Amount < 0);
				
				// Delete the newest zero or less transaction
				if (firstPagDebitTransactionEntities != default)
				{
					// TO DO: This is a hacky way to do this. Need to re-add game association so that we can remove for the specific game.
					await transactionService.DeleteTransactionEntityAsync(firstPagDebitTransactionEntities.RowKey);
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
			if (pagEntity.PlayerRowKey is null) return;
			if (pagEntity.GameRowKey is null) return;
			
			// Set the UrlSegment
			// Cannot do this as setter for UrlSegment because we cannot resolve the GameEntity and PlayerEntity there
			var playerEntity = playerService.GetPlayerEntity(pagEntity.PlayerRowKey);
			var gameEntity = GetGameEntity(pagEntity.GameRowKey);
			pagEntity.UrlSegment = $"{playerEntity.UrlSegment}-{gameEntity.UrlSegment}";
			
			// Upsert pag
			await dataService.UpsertEntityAsync(pagEntity);
		}

		private List<ExpandedPag> ExpandPags(List<PlayerAtGameEntity> pags)
		{
			List<ExpandedPag> ePags = (from pag in pags 
				let ge = GetGameEntity(pag.GameRowKey) 
				let pe = playerService.GetPlayerEntity(pag.PlayerRowKey) 
				select new ExpandedPag(pag, ge, pe)).ToList();
			return ePags;
		}
	}

	public class ExpandedPag(PlayerAtGameEntity pagEntity, GameEntity gameEntity, PlayerEntity playerEntity)
	{
		public PlayerAtGameEntity PagEntity { get; } = pagEntity;

		public PlayerEntity PlayerEntity { get; } = playerEntity;

		public GameEntity GameEntity { get; set; } = gameEntity;
	}
}
