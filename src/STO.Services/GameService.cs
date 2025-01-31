using STO.Models;
using STO.Models.Interfaces;

namespace STO.Services;

public class GameService(IDataService dataService) : IGameService
{
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