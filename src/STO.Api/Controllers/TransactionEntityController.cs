using Microsoft.AspNetCore.Mvc;
using STO.Models;
using STO.Models.Interfaces;
using STO.Services;

namespace STO.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionEntityController : ControllerBase
{
    private readonly ILogger<TransactionEntityController> _logger;

    private readonly IStorageService _storageService;

    public TransactionEntityController(ILogger<TransactionEntityController> logger, IStorageService storageService)
    {
        _logger = logger;
        _storageService = storageService;
    }

    [HttpGet(Name = "GetTransactionEntitys")]
    public async Task<IEnumerable<TransactionEntity>> Get()
    {
        var transactionEntitiesResult = await _storageService.QueryEntities<TransactionEntity>();
        var transactionEntities = transactionEntitiesResult.ToList();

        return transactionEntities;
    }

    [HttpDelete(Name = "DeleteTransactionEntity")]
    public async Task Delete(string rowkey)
    {
        await _storageService.DeleteEntity<TransactionEntity>(rowkey);
    }

    [HttpPost(Name = "UpsertTransactionEntity")]
    public async Task Post(TransactionEntity TransactionEntity)
    {
        await _storageService.UpsertEntity<TransactionEntity>(TransactionEntity);
    }
}