using STO.Models;
using STO.Models.Interfaces;

namespace STO.Services;

public class PlayerAtGameService : IPlayerAtGameService
{
    private readonly IEnumerable<PlayerAtGameEntity> _pagEntities;
    private readonly IEnumerable<GameEntity> _gameEntities;
    private readonly IEnumerable<PlayerEntity> _playerEntities;

    public PlayerAtGameService(IDataService dataService)
    {
        _pagEntities = dataService.PlayerAtGameEntities.OrderByDescending(o => o.UrlSegment);
        _gameEntities = dataService.GameEntities.OrderByDescending(o => o.Date);
        _playerEntities = dataService.PlayerEntities.OrderByDescending(o => o.Name);
    }

    private PlayerAtGame ConstructPag(string pagId)
    {
        var pag = new PlayerAtGame();
        
        return pag;
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
            Team = pag.Team,
            UrlSegment = pag.UrlSegment,
        };

        return pe;
    }

    public List<PlayerAtGame> GetPags(int? skip, int? take)
    {
        throw new NotImplementedException();
    }

    public List<PlayerAtGame> GetPagsForGame(string gameId)
    {
        throw new NotImplementedException();
    }

    public List<PlayerAtGame> OrganiseTeams(List<PlayerAtGame> pags)
    {
        throw new NotImplementedException();
    }

    public async Task ResetTeamsAsync(List<PlayerAtGame> pags)
    {
        throw new NotImplementedException();
    }

    public async Task TogglePagPlayedAsync(string pagId, bool played)
    {
        throw new NotImplementedException();
    }

    public Game GetPag(string id)
    {
        throw new NotImplementedException();
    }

    public Game GetPagByUrlSegment(string urlSegment)
    {
        throw new NotImplementedException();
    }

    public async Task DeletePagAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task UpsertPagAsync(PlayerAtGame pag)
    {
        throw new NotImplementedException();
    }
}