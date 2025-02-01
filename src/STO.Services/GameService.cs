using STO.Models;
using STO.Models.Interfaces;

namespace STO.Services;

public class GameService : IGameService
{
    private readonly IDataService _dataService;
    private readonly List<GameEntity> _gameEntities;

    public GameService(IDataService dataService)
    {
        _dataService = dataService;
        _gameEntities = _dataService.GameEntities.OrderByDescending(o => o.Date).ToList();
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
            throw new KeyNotFoundException($"The GameEntity with ID {gameEntity} was not found.");
        }

        return gameEntity;
    }

    public List<Game> GetGames()
    {
        throw new NotImplementedException();
    }

    public Game GetGame(string id)
    {
        throw new NotImplementedException();
    }

    public Game GetGameByUrlSegment(string urlSegment)
    {
        throw new NotImplementedException();
    }

    public Game GetNextGame()
    {
        throw new NotImplementedException();
    }

    public async Task DeleteGameAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task UpsertGameAsync(Game game)
    {
        throw new NotImplementedException();
    }
}