using Microsoft.AspNetCore.Mvc;
using STO.Models;
using STO.Models.Interfaces;
using STO.Services;

namespace STO.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RatingEntityController : ControllerBase
{
    private readonly ILogger<RatingEntityController> _logger;

    private readonly IStorageService _storageService;

    public RatingEntityController(ILogger<RatingEntityController> logger, IStorageService storageService)
    {
        _logger = logger;
        _storageService = storageService;
    }

    [HttpGet(Name = "GetRatingEntitys")]
    public IEnumerable<RatingEntity> Get()
    {
        var ratingEntities = _storageService.QueryEntities<RatingEntity>().ToList();

        return ratingEntities;
    }

    [HttpDelete(Name = "DeleteRatingEntity")]
    public void Delete(string rowkey)
    {
        _storageService.DeleteEntity<RatingEntity>(rowkey);
    }

    [HttpPost(Name = "UpsertRatingEntity")]
    public async Task Post(RatingEntity RatingEntity)
    {
        await _storageService.UpsertEntity<RatingEntity>(RatingEntity);
    }
}