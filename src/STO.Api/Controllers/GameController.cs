using Microsoft.AspNetCore.Mvc;
using STO.Models;
using STO.Models.Interfaces;
using STO.Api.Services;

namespace STO.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;

    private readonly IStorageService _storageService;

    public GameController(ILogger<GameController> logger, IStorageService storageService)
    {
        _logger = logger;
        _storageService = storageService;
    }

    [HttpGet(Name = "GetGames")]
    public IEnumerable<GameEntity> Get()
    {
        var gameEntities = _storageService.QueryEntities<GameEntity>()
            .OrderByDescending(g => g.Date)
            .ToList();

        return gameEntities;
    }
}