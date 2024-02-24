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
    public IEnumerable<TransactionEntity> Get()
    {
        var transactionEntities = _storageService.QueryEntities<TransactionEntity>().ToList();

        return transactionEntities;
    }

    [HttpDelete(Name = "DeleteTransactionEntity")]
    public void Delete(string rowkey)
    {
        _storageService.DeleteEntity<TransactionEntity>(rowkey);
    }

    [HttpPost(Name = "UpsertTransactionEntity")]
    public async Task Post(TransactionEntity TransactionEntity)
    {
        await _storageService.UpsertEntity<TransactionEntity>(TransactionEntity);
    }
}