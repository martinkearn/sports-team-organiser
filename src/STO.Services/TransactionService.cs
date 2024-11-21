using System.Globalization;
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

    public List<Transaction> GetTransactions()
    {
        throw new NotImplementedException();
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