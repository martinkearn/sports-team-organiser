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
        // Get Transaction
        var te = VerifyTransaction(transactionId);
        
        // Get PlayerEntity
        var pe = dataService.PlayerEntities.Single(p => p.RowKey == te.PlayerRowKey);
        
        // Get GameEntity
        //var ge = dataService.GameEntities.Single(g => g.RowKey == te.)
        throw new NotImplementedException();
    }

    private TransactionEntity VerifyTransaction(string transactionId)
    {
        var transactionEntity = GetTransactionEntities().FirstOrDefault(pe => pe.RowKey == transactionId);
        if (transactionEntity == default)
        {
            throw new KeyNotFoundException($"The TransactionEntity with ID {transactionId} was not found.");
        }

        return transactionEntity;
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
        throw new NotImplementedException();
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