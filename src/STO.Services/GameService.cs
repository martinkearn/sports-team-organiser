using STO.Models;
using STO.Models.Interfaces;

namespace STO.Services;

public class GameService : IGameService
{
    private readonly IDataService _dataService;
    private readonly IEnumerable<GameEntity> _gameEntities;

    public GameService(IDataService dataService)
    {
        _dataService = dataService;
        _gameEntities = _dataService.GameEntities.OrderByDescending(o => o.Date);
    }

    private Game ConstructGame(string gameId)
    {
        // Verify Game
        var gameEntity = VerifyGameExists(gameId);
        
        // Construct
        var game = new Game()
        {
            Id = gameId,
            DateTime = gameEntity.Date.DateTime,
            TeamAGoals = gameEntity.TeamAGoals,
            TeamBGoals = gameEntity.TeamBGoals,
            Title = gameEntity.Title,
            Notes = gameEntity.Notes,
            //Label is filled by the Game class
            //UrlSegment is filled by the game class
        };
        
        return game;
    }

    private GameEntity DeconstructGame(Game game)
    {
        var ge = new GameEntity()
        {
            RowKey = game.Id,
            Date = game.DateTime,
            TeamAGoals = game.TeamAGoals,
            TeamBGoals = game.TeamBGoals,
            Title = game.Title,
            Notes = game.Notes,
            UrlSegment = game.UrlSegment
        };
        
        return ge;
    }
    
    private GameEntity VerifyGameExists(string gameId)
    {
        var gameEntity = _gameEntities.FirstOrDefault(ge => ge.RowKey == gameId);
        if (gameEntity == null)
        {
            throw new KeyNotFoundException($"The Game with Id {gameId} was not found.");
        }

        return gameEntity;
    }

    public List<Game> GetGames(int? skip, int? take)
    {
        var ges = _gameEntities;
        
        // Apply Skip and Take only if values are provided
        ges = skip.HasValue ? ges.Skip(skip.Value) : ges;
        ges = take.HasValue ? ges.Take(take.Value) : ges;

        var gs = ges.Select(ge => ConstructGame(ge.RowKey)).ToList();

        return gs;
    }

    public Game GetGame(string id)
    {
        return ConstructGame(id);  
    }

    public Game GetGameByUrlSegment(string urlSegment)
    {
        // Get PlayerEntity for this UrlSegment
        var ge = _dataService.GameEntities.FirstOrDefault(ge => ge.UrlSegment.ToLowerInvariant() == urlSegment.ToLowerInvariant());

        if (ge == null)
        {
            throw new KeyNotFoundException($"The game with UrlSegment {urlSegment} was not found.");
        }

        return ConstructGame(ge.RowKey);
    }

    public Game GetNextGame()
    {
        var nextGe = _dataService.GameEntities.OrderByDescending(o => o.Date.DateTime).FirstOrDefault();

        if (nextGe == null)
        {
            throw new KeyNotFoundException("No next game found.");
        }

        return ConstructGame(nextGe.RowKey);
    }

    public async Task DeleteGameAsync(string id)
    {
        // Verify Game
        var gameEntity = VerifyGameExists(id);
        
        // Delete Ratings
        var ratingsForGame = _dataService.RatingEntities.Where(re => re.GameRowKey == id);
        foreach (var re in ratingsForGame)
        {
            await _dataService.DeleteEntityAsync<RatingEntity>(re.RowKey);
        }

        // Delete PAGs
        var pagsForGame = _dataService.PlayerAtGameEntities.Where(pag => pag.GameRowKey == id);
        foreach (var pag in pagsForGame)
        {
            await _dataService.DeleteEntityAsync<PlayerAtGameEntity>(pag.RowKey);
        }

        // Delete game
        await _dataService.DeleteEntityAsync<GameEntity>(id);
    }

    public async Task UpsertGameAsync(Game game)
    {
        var ge = DeconstructGame(game);
        await _dataService.UpsertEntityAsync(ge);
    }
}