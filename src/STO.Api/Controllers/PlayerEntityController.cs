using Microsoft.AspNetCore.Mvc;

namespace STO.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerEntityController : ControllerBase
{
    private readonly ILogger<PlayerEntityController> _logger;

    private readonly IStorageService _storageService;

    public PlayerEntityController(ILogger<PlayerEntityController> logger, IStorageService storageService)
    {
        _logger = logger;
        _storageService = storageService;
    }

    [HttpGet(Name = "GetPlayerEntitys")]
    public async Task<IEnumerable<PlayerEntity>> Get()
    {
        var playerEntitiesResult = await _storageService.QueryEntities<PlayerEntity>();
        var playerEntities = playerEntitiesResult
            .OrderBy(p => p.Name)
            .ToList();

        return playerEntities;
    }

    [HttpDelete(Name = "DeletePlayerEntity")]
    public async Task Delete(string rowkey)
    {
        await _storageService.DeleteEntity<PlayerEntity>(rowkey);
    }

    [HttpPost(Name = "UpsertPlayerEntity")]
    public async Task Post(PlayerEntity playerEntity)
    {
        await _storageService.UpsertEntity<PlayerEntity>(playerEntity);
    }
}