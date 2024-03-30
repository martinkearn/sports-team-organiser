using Microsoft.AspNetCore.Mvc;

namespace STO.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DataDetailsEntityController : ControllerBase
{
    private readonly ILogger<GameEntityController> _logger;

    private readonly IStorageService _storageService;

    public DataDetailsEntityController(ILogger<GameEntityController> logger, IStorageService storageService)
    {
        _logger = logger;
        _storageService = storageService;
    }

    [HttpGet(Name = "GetDataDetailsEntity")]
    public async Task<IEnumerable<DataDetailsEntity>> Get()
    {
        var dataDetailsEntity = await _storageService.QueryEntities<DataDetailsEntity>();
        return dataDetailsEntity;
    }
}