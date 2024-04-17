using Microsoft.AspNetCore.Mvc;

namespace STO.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DataDetailsEntityController(ILogger<GameEntityController> logger, IStorageService storageService) : ControllerBase
{
    private readonly ILogger<GameEntityController> _logger = logger;

    private readonly IStorageService _storageService = storageService;

	[HttpGet(Name = "GetDataDetailsEntity")]
    public async Task<IEnumerable<DataDetailsEntity>> Get()
    {
        var dataDetailsEntity = await _storageService.QueryEntities<DataDetailsEntity>();
        return dataDetailsEntity;
    }

	[HttpPost(Name = "PostDataDetailsEntity")]
	public async Task Post()
	{
		// Create DataDetailsEntity
		var dataDetailsEntity = new DataDetailsEntity();

		// Upsert entity
		await _storageService.UpsertEntity<DataDetailsEntity>(dataDetailsEntity);
	}
}