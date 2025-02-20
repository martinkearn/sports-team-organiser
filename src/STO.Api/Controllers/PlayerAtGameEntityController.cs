using Microsoft.AspNetCore.Mvc;

namespace STO.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerAtGameEntityController : ControllerBase
{
    private readonly ILogger<PlayerAtGameEntityController> _logger;

    private readonly IStorageService _storageService;

    public PlayerAtGameEntityController(ILogger<PlayerAtGameEntityController> logger, IStorageService storageService)
    {
        _logger = logger;
        _storageService = storageService;
    }

    [HttpGet(Name = "GetPlayerAtGameEntitys")]
    public async Task<IEnumerable<PlayerAtGameEntity>> Get()
    {
        var playerEntitiesResult = await _storageService.QueryEntities<PlayerAtGameEntity>();
        var playerEntities = playerEntitiesResult.ToList();
        return playerEntities;
    }

    [HttpDelete(Name = "DeletePlayerAtGameEntity")]
    public async Task Delete(string rowkey)
    {
        await _storageService.DeleteEntity<PlayerAtGameEntity>(rowkey);
    }

    [HttpPost(Name = "UpsertPlayerAtGameEntity")]
    public async Task Post(PlayerAtGameEntity playerAtGameEntity)
    {
        await _storageService.UpsertEntity<PlayerAtGameEntity>(playerAtGameEntity);
    }
}