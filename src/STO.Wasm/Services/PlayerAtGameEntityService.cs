namespace STO.Wasm.Services;

public class PlayerAtGameEntityService(IDataService dataService, IRatingEntityService ratingEntityEntityService, ITransactionService transactionService) : IPlayerAtGameEntityService
{
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

    public List<PlayerAtGameEntity> GetPlayerAtGameEntities()
    {
        return dataService.PlayerAtGameEntities.OrderByDescending(o => o.Timestamp!.Value.DateTime).ToList();
    }

    public PlayerAtGameEntity GetPlayerAtGameEntityByUrlSegment(string urlSegment)
    {
        return dataService.PlayerAtGameEntities.First(o => o.UrlSegment == urlSegment);
    }

    public async Task UpsertPlayerAtGameEntityAsync(PlayerAtGameEntity pagEntity)
    {
	    if (pagEntity.PlayerRowKey is null) return;
	    if (pagEntity.GameRowKey is null) return;
			
	    // Set the UrlSegment
	    // Cannot do this as setter for UrlSegment because we cannot resolve the GameEntity and PlayerEntity there
	    var playerEntity = dataService.PlayerEntities.First(pe => pe.RowKey == pagEntity.PlayerRowKey);
	    var gameEntity = dataService.GameEntities.First(ge => ge.RowKey == pagEntity.GameRowKey);
	    pagEntity.UrlSegment = $"{playerEntity.UrlSegment}-{gameEntity.UrlSegment}";
			
	    // Check if PAG exists
	    var matchingPagsByUrlSegment =
		    dataService.PlayerAtGameEntities.Count(p => p.UrlSegment == pagEntity.UrlSegment);

	    if (matchingPagsByUrlSegment > 0)
	    {
		    var matchingPag = GetPlayerAtGameEntityByUrlSegment(pagEntity.UrlSegment);
		    matchingPag.Forecast = pagEntity.Forecast;
		    matchingPag.Played = pagEntity.Played;
		    matchingPag.Team = pagEntity.Team;
				
		    // Upsert pag
		    await dataService.UpsertEntityAsync(matchingPag);
	    }
	    else
	    {
		    // Upsert pag
		    await dataService.UpsertEntityAsync(pagEntity);
	    }
    }

    public async Task DeletePlayerAtGameEntityAsync(string rowKey)
    {
        await dataService.DeleteEntityAsync<PlayerAtGameEntity>(rowKey);
    }

    public async Task<List<PlayerAtGameEntity>> CalculateTeamsAsync(List<PlayerAtGameEntity> pags)
    {
	    // Resolve Pags to ExpandedPags
	    var ePags = ExpandPags(pags);

	    // Get Yes pags
	    var newEPags = new List<ExpandedPag>();
	    var yesPags = ePags
		    .Where(o => o.PagEntity.Forecast == Enums.PlayingStatus.Yes)
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

		public async Task TogglePlayerAtGamePlayedAsync(PlayerAtGameEntity pag, bool? played)
		{
			// Get player for pag
			var playerEntity = dataService.PlayerEntities.First(pe => pe.RowKey == pag.PlayerRowKey);

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
				var chargeTransaction = new Transaction()
				{
					PlayerId = pag.PlayerRowKey,
					Amount = -playerEntity.DefaultRate,
					DateTime = DateTime.UtcNow,
					GameId = pag.GameRowKey
				};

                await transactionService.UpsertTransactionAsync(chargeTransaction);
			}
			else
			{
				// Get debit transactions (less than £0) for player and game
				var chargeTransactionsForPlayer = transactionService.GetTransactions(null, null, pag.PlayerRowKey)
					.Where(t => t.Amount < 0)
					.ToList();
				
				// Do we have any associated with this game. If so, use those
				var chargeTransactionsForPag = chargeTransactionsForPlayer.Where(t => t.GameId == pag.GameRowKey).ToList();
				if (chargeTransactionsForPag.Count > 0)
				{
					foreach (var t in chargeTransactionsForPag)
					{
						await transactionService.DeleteTransactionAsync(t.Id);
					}
				}
				else
				{
					// If not, these must be old transactions without the GameId. Just remove the latest one
					var latestTransaction = chargeTransactionsForPlayer.OrderByDescending(o => o.DateTime).First();
					await transactionService.DeleteTransactionAsync(latestTransaction.Id);
				}
			}

			// Upsert pag
			await UpsertPlayerAtGameEntityAsync(pag);
		}

    public string GetPlayerAtGameLabel(string rowKey, Enums.TitleLength length = Enums.TitleLength.Short)
    {
        // Get expanded PAG
        var pag = GetPlayerAtGameEntity(rowKey);
        var pagInList = new List<PlayerAtGameEntity> { pag };
        var expandedPags = ExpandPags(pagInList);
        var expandedPag = expandedPags.First();
        
        // Construct label
        //TODO: When there is a .Label object on Game, use it here.
        var gameLabel = expandedPag.GameEntity.Date.ToString("dd MMM");
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
    
    private List<ExpandedPag> ExpandPags(List<PlayerAtGameEntity> pags)
    {
	    List<ExpandedPag> ePags = (from pag in pags 
		    let ge = dataService.GameEntities.FirstOrDefault(ge => ge.RowKey == pag.GameRowKey) 
		    let playerEntity = dataService.PlayerEntities.FirstOrDefault(pe => pe.RowKey == pag.PlayerRowKey)
		    select new ExpandedPag(pag, ge, playerEntity)).ToList();
	    return ePags;
    }
}

public class ExpandedPag(PlayerAtGameEntity pagEntity, GameEntity gameEntity, PlayerEntity playerEntity)
{
	public PlayerAtGameEntity PagEntity { get; } = pagEntity;
	public PlayerEntity PlayerEntity { get; } = playerEntity;
	public GameEntity GameEntity { get; set; } = gameEntity;
}