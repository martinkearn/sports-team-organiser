using System.Globalization;
using STO.Models;
using STO.Models.Interfaces;

namespace STO.Services;

public class TransactionService(IDataService dataService) : ITransactionService
{
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