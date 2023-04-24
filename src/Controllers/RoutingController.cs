using Microsoft.AspNetCore.Mvc;

[Route("controllers/[controller]/[action]")]
public class RoutingController : Controller
{
    IGameService _gameService;
    public RoutingController(IGameService gameService)
    {
        _gameService = gameService;
    }

    public async Task<IActionResult> LatestGame(int id)
    {
        var games = await _gameService.GetGames();
        var latestGame = games.OrderByDescending(o => o.GameEntity.Date).FirstOrDefault();
        return Redirect($"/games/{latestGame.GameEntity.RowKey}");
    }
}