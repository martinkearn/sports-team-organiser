using Microsoft.AspNetCore.Mvc;
using STO.Models;
using STO.Models.Interfaces;
using STO.Services;

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
    public IEnumerable<PlayerEntity> Get()
    {
        var playerEntities = _storageService.QueryEntities<PlayerEntity>()
            .OrderBy(p => p.Name)
            .ToList();

        return playerEntities;
    }

    [HttpDelete(Name = "DeletePlayerEntity")]
    public void Delete(string rowkey)
    {
        _storageService.DeleteEntity<PlayerEntity>(rowkey);
    }

    [HttpPost(Name = "UpsertPlayerEntity")]
    public async Task Post(PlayerEntity PlayerEntity)
    {
        await _storageService.UpsertEntity<PlayerEntity>(PlayerEntity);
    }
}