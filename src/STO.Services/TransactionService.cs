using STO.Models;
using STO.Models.Interfaces;

namespace STO.Services;

public class TransactionService(IDataService dataService) : ITransactionService
{
    
    private IEnumerable<TransactionEntity> GetTransactionEntities()
    {
        return [.. dataService.TransactionEntities.OrderBy(o => o.Date)];
    }

    private Transaction ConstructTransaction(string transactionId)
    {
        // Get TransactionEntity
        var te = GetTransactionEntities().Single(te => te.RowKey == transactionId);

        // Get PlayerEntity
        var pe = dataService.PlayerEntities.Single(p => p.RowKey == te.PlayerRowKey);

        // Get GameEntity
        var ge = dataService.GameEntities.SingleOrDefault(g => g.RowKey == te.GameRowKey);

        // Construct Transaction
        var t = new Transaction(te.RowKey, pe, ge)
        {
            Amount = te.Amount,
            DateTime = te.Date.DateTime,
            Notes = te.Notes,
        };

        return t;
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

    public Task DeleteTransactionAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task UpsertTransactionAsync(Transaction transaction)
    {
        throw new NotImplementedException();
    }
}