using Microsoft.AspNetCore.Mvc;
using STO.Models;
using STO.Models.Interfaces;
using STO.Services;

namespace STO.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GameEntityController : ControllerBase
{
    private readonly ILogger<GameEntityController> _logger;

    private readonly IStorageService _storageService;

    public GameEntityController(ILogger<GameEntityController> logger, IStorageService storageService)
    {
        _logger = logger;
        _storageService = storageService;
    }

    [HttpGet(Name = "GetGameEntitys")]
    public async Task<IEnumerable<GameEntity>> Get()
    {
        var gameEntitiesResult = await _storageService.QueryEntities<GameEntity>();
        var gameEntities = gameEntitiesResult
            .OrderByDescending(g => g.Date)
            .ToList();

        return gameEntities;
    }

    [HttpDelete(Name = "DeleteGameEntity")]
    public async Task Delete(string rowkey)
    {
        await _storageService.DeleteEntity<GameEntity>(rowkey);
    }

    [HttpPost(Name = "UpsertGameEntity")]
    public async Task Post(GameEntity gameEntity)
    {
        await _storageService.UpsertEntity<GameEntity>(gameEntity);
    }
}