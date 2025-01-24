using STO.Models;
using STO.Models.Interfaces;

namespace STO.Services;

public class TransactionService(IDataService dataService) : ITransactionService
{
    
    private IEnumerable<TransactionEntity> GetTransactionEntities()
    {
        return [.. dataService.TransactionEntities.OrderByDescending(o => o.Date.DateTime)];
    }

    private Transaction ConstructTransaction(string transactionId)
    {
        // Get TransactionEntity
        var te = GetTransactionEntities().SingleOrDefault(te => te.RowKey == transactionId);
        
        if (te == default)
        {
            throw new KeyNotFoundException();
        }

        // Get PlayerEntity
        var pe = dataService.PlayerEntities.Single(p => p.RowKey == te.PlayerRowKey);

        // Get GameEntity
        var ge = dataService.GameEntities.SingleOrDefault(g => g.RowKey == te.GameRowKey);

        // Construct Transaction
        var t = new Transaction()
        {
            Id = te.RowKey,
            Amount = te.Amount,
            DateTime = te.Date.DateTime,
            Notes = te.Notes,
            PlayerId = pe.RowKey,
            PlayerName = pe.Name,
            PlayerUrlSegment = pe.UrlSegment
        };
        if (ge != null)
        {
            t.GameId = ge.RowKey;
            t.GameLabel = ge.Title;
        }

        return t;
    }
    
    private static TransactionEntity DeconstructTransaction(Transaction transaction)
    {
        var te = new TransactionEntity()
        {
            RowKey = transaction.Id,
            Amount = transaction.Amount,
            Date = new DateTimeOffset(transaction.DateTime),
            GameRowKey = transaction.GameId,
            Notes = transaction.Notes,
            PlayerRowKey = transaction.PlayerId,
            UrlSegment = transaction.UrlSegment
        };
        return te;
    }

    public List<Transaction> GetTransactions(int? skip, int? take)
    {
        // Get all TransactionEntity
        var tes = GetTransactionEntities();
        
        // Apply Skip and Take only if values are provided
        tes = skip.HasValue ? tes.Skip(skip.Value) : tes;
        tes = take.HasValue ? tes.Take(take.Value) : tes;

        var ts = tes.Select(te => ConstructTransaction(te.RowKey)).ToList();

        return ts;
    }
    
    public List<Transaction> GetTransactions(int? skip, int? take, string playerId)
    {
        // If playerId not defined, do not filter by player
        if (string.IsNullOrEmpty(playerId))
        {
            return GetTransactions(skip, take);
        }

        // Get player's TransactionEntity
        var tes = GetTransactionEntities().Where(te => te.PlayerRowKey == playerId);
        
        // Apply Skip and Take only if values are provided
        tes = skip.HasValue ? tes.Skip(skip.Value) : tes;
        tes = take.HasValue ? tes.Take(take.Value) : tes;

        var ts = tes.Select(te => ConstructTransaction(te.RowKey)).ToList();

        return ts;
    }

    public List<Transaction> GetTransactions(string playerId)
    {
        throw new NotImplementedException();
    }

    public Transaction GetTransaction(string id)
    {
        return ConstructTransaction(id);
    }

    public Transaction GetTransactionByUrlSegment(string urlSegment)
    {
        // Get TransactionEntity for this UrlSegment
        var te = GetTransactionEntities().FirstOrDefault(te => te.UrlSegment == urlSegment);

        if (te == default)
        {
            throw new KeyNotFoundException();
        }

        return ConstructTransaction(te!.RowKey);
    }

    public async Task DeleteTransactionAsync(string id)
    {
        await dataService.DeleteEntityAsync<TransactionEntity>(id);
    }

    public async Task UpsertTransactionAsync(Transaction transaction)
    {
        var te = DeconstructTransaction(transaction);
        await dataService.UpsertEntityAsync(te);
    }
}