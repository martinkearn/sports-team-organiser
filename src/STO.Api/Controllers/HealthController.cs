using Microsoft.AspNetCore.Mvc;

namespace STO.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<GameEntityController> _logger;

    private readonly IStorageService _storageService;

    public HealthController(ILogger<GameEntityController> logger, IStorageService storageService)
    {
        _logger = logger;
        _storageService = storageService;
    }

    [HttpPut(Name = "PutLatestData")]
    public async Task Put()
    {
        await _storageService.RefreshData();
    }

    [HttpGet(Name = "GetDataDetails")]
    public async Task<DataDetailsEntity> Get()
    {
        var dataDetailsEntity = await _storageService.QueryEntities<DataDetailsEntity>();
        return dataDetailsEntity.FirstOrDefault();
    }
}