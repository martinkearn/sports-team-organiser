using STO.Models;
using STO.Models.Interfaces;

namespace STO.Services;

public class PlayerAtGameService : IPlayerAtGameService
{
    private readonly IEnumerable<PlayerAtGameEntity> _pagEntities;
    private readonly IDataService _dataService;
    private readonly IPlayerService _playerService;
    private readonly IGameService _gameService;
    private readonly ITransactionService _transactionService;

    public PlayerAtGameService(IDataService dataService, IGameService gameService, IPlayerService playerService, ITransactionService transactionService)
    {
        _pagEntities = dataService.PlayerAtGameEntities.OrderByDescending(o => o.UrlSegment);
        _dataService = dataService;
        _playerService = playerService;
        _gameService = gameService;
        _transactionService = transactionService;
    }

    private PlayerAtGame ConstructPag(string pagId)
    {
        // Verify Pag
        var pagEntity = VerifyPagEntityExists(pagId);
        
        // Get associated objects
        var player = _playerService.GetPlayer(pagEntity.PlayerRowKey);
        var game = _gameService.GetGame(pagEntity.GameRowKey);

        // Get Team
        var team = Enums.Team.None;
        if (!string.IsNullOrEmpty(pagEntity.Team))
        {
            team = (Enums.Team)Enum.Parse(typeof(Enums.Team), pagEntity.Team);
        }

        // Construct
        var pag = new PlayerAtGame()
        {
            Id = pagEntity.RowKey,
            PlayerId = pagEntity.PlayerRowKey,
            GameId = pagEntity.GameRowKey,
            Forecast = pagEntity.Forecast,
            Played = pagEntity.Played,
            Team = team,
            PlayerLabel = player.Label,
            PlayerBalance = player.Balance,
            PlayerName = player.Name,
            PlayerPosition = player.Position,
            PlayerRating = player.Rating,
            PlayerAdminRating = player.AdminRating,
            PlayerDefaultRate = player.DefaultRate,
            PlayerGamesCount = player.GamesCount,
            PlayerUrlSegment = player.UrlSegment,
            GameLabel = game.Label,
            GameDateTime = game.DateTime,
            GameUrlSegment = game.UrlSegment,
            UrlSegment = $"{player.UrlSegment}-{game.UrlSegment}"
            //Label is filled in the PlayerAtGame class
        };
        
        return pag;
    }
    
    private PlayerAtGameEntity VerifyPagEntityExists(string pagId)
    {
        var pagEntity = _pagEntities.FirstOrDefault(page => page.RowKey == pagId);
        if (pagEntity == null)
        {
            throw new KeyNotFoundException($"The PlayerAtGame with Id {pagId} was not found.");
        }
        return pagEntity;
    }

    private PlayerAtGameEntity DeconstructPag(PlayerAtGame pag)
    {
        var pe = new PlayerAtGameEntity()
        {
            RowKey = pag.Id,
            PlayerRowKey = pag.PlayerId,
            GameRowKey = pag.GameId,
            Forecast = pag.Forecast,
            Played = pag.Played,
            Team = pag.Team.ToString(),
            UrlSegment = pag.UrlSegment,
        };

        return pe;
    }

    public List<PlayerAtGame> GetPags(int? skip, int? take)
    {
        var pagEntities = _pagEntities;
        
        // Apply Skip and Take only if values are provided
        pagEntities = skip.HasValue ? pagEntities.Skip(skip.Value) : pagEntities;
        pagEntities = take.HasValue ? pagEntities.Take(take.Value) : pagEntities;

        var pags = pagEntities.Select(page => ConstructPag(page.RowKey)).ToList();

        return pags;
    }

    public List<PlayerAtGame> GetPagsForGame(string gameId)
    {
        if (string.IsNullOrEmpty(gameId))
        {
            throw new ArgumentNullException(nameof(gameId));
        }
        
        var pags = _pagEntities
            .Where(page => page.GameRowKey == gameId)
            .Select(page => ConstructPag(page.RowKey)).ToList();
        return pags;
    }

    public async Task<List<PlayerAtGame>> OrganiseTeams(List<PlayerAtGame> pags)
    {
        // Filter only players who are actually playing
        var availablePlayers = pags.Where(p => p.Forecast == Enums.PlayingStatus.Yes).ToList();

        // Dictionary to hold players sorted by position
        var groupedByPosition = availablePlayers
            .GroupBy(p => p.PlayerPosition)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(p => p.PlayerAdminRating).ToList());

        var teamA = new List<PlayerAtGame>();
        var teamB = new List<PlayerAtGame>();
        var updateTasks = new List<Task>(); // List to batch async updates

        // Assign players in alternating order by position
        foreach (var positionGroup in groupedByPosition)
        {
            var assignToTeamA = true;

            foreach (var player in positionGroup.Value)
            {
                if (assignToTeamA)
                    teamA.Add(player);
                else
                    teamB.Add(player);

                // Toggle team for next player
                assignToTeamA = !assignToTeamA;
            }
        }

        // Assign team values
        foreach (var pagA in teamA)
        {
            pagA.Team = Enums.Team.A;
            updateTasks.Add(UpsertPagAsync(pagA));
        }

        foreach (var pagB in teamB)
        {
            pagB.Team = Enums.Team.B;
            updateTasks.Add(UpsertPagAsync(pagB));
        }
        
        // Wait for all async updates to complete
        await Task.WhenAll(updateTasks);

        return teamA.Concat(teamB).OrderBy(p => p.PlayerName).ToList(); // Return sorted for consistency
    }

    public async Task ResetTeamsAsync(List<PlayerAtGame> pags)
    {
        // Using a For loop because we are modifying the collection as we go
        var index = 0;
        for (; index < pags.Count; index++)
        {
            var pag = pags[index];
            pag.Team = Enums.Team.None;
            await UpsertPagAsync(pag);
        }
    }

    public async Task TogglePagPlayedAsync(string pagId)
    {
        // Get Pag
        var pag = GetPag(pagId);
        
        // Just toggle the pag value
        pag.Played = !pag.Played;
        
        // Add / remove transactions if played / not played
        if (pag.Played)
        {
            var chargeTransaction = new Transaction()
            {
                PlayerId = pag.PlayerId,
                Amount = -pag.PlayerDefaultRate,
                DateTime = DateTime.UtcNow,
                GameId = pag.GameId
            };
            
            await _transactionService.UpsertTransactionAsync(chargeTransaction);
        }
        else
        {
            // Get debit transactions (less than £0) for player and game
            var chargeTransactionsForPlayer = _transactionService.GetTransactions(null, null, pag.PlayerId)
                .Where(t => t.Amount < 0)
                .ToList();
				
            // Do we have any associated with this game? If so, use those
            var chargeTransactionsForPag = chargeTransactionsForPlayer.Where(t => t.GameId == pag.GameId).ToList();
            if (chargeTransactionsForPag.Count > 0)
            {
                foreach (var t in chargeTransactionsForPag)
                {
                    await _transactionService.DeleteTransactionAsync(t.Id);
                }
            }
            else
            {
                // If not, these must be old transactions without the GameId. Add a credit to balance it out. Cannot delete any because we do not know which game they are assoicated with
                var creditTransaction = new Transaction()
                {
                    PlayerId = pag.PlayerId,
                    Amount = pag.PlayerDefaultRate,
                    DateTime = DateTime.UtcNow,
                    GameId = pag.GameId
                };
                
                await _transactionService.UpsertTransactionAsync(creditTransaction);
            }
        }

        // Upsert pag
        await UpsertPagAsync(pag);
    }

    public PlayerAtGame GetPag(string id)
    {
        return ConstructPag(id);
    }

    public PlayerAtGame GetPagByUrlSegment(string urlSegment)
    {
        ArgumentNullException.ThrowIfNull(urlSegment);
        
        // Trim whitespace
        urlSegment = urlSegment.Trim();

        // Get PlayerAtGameEntity for this UrlSegment
        var page = _pagEntities.FirstOrDefault(page => page.UrlSegment.ToLowerInvariant() == urlSegment.ToLowerInvariant());

        if (page == null)
        {
            throw new KeyNotFoundException($"The PlayerAtGame with UrlSegment {urlSegment} was not found.");
        }
        
        return ConstructPag(page.RowKey);
    }

    public async Task DeletePagAsync(string id)
    {
        await _dataService.DeleteEntityAsync<PlayerAtGameEntity>(id);
    }

    public async Task UpsertPagAsync(PlayerAtGame pag)
    {
        if (pag.PlayerId is null) return;
        if (pag.GameId is null) return;
        
        //Deconstruct
        var pagEntity = DeconstructPag(pag);
        
        //Set UrlSegment
        pagEntity.UrlSegment = $"{pag.PlayerUrlSegment}-{pag.GameUrlSegment}";
        
        //Check if Pag exists with same GameId and PlayerID.
        //We could have Pags with different ID but same Player and Game if a Pag has been already added to Game
        var matchingPagEntity = _pagEntities.FirstOrDefault(page => page.UrlSegment == pagEntity.UrlSegment);
        if (matchingPagEntity is not null)
        {
            //Overwrite the PagEntity we created with a new one based on existing PagEntity details with new properties from incoming Pag
            pagEntity = matchingPagEntity;
            pagEntity.Team = pag.Team.ToString();
            pagEntity.Forecast = pag.Forecast;
            pagEntity.Played = pag.Played;
        }
        
        // Upsert pag
        await _dataService.UpsertEntityAsync(pagEntity);
    }
}