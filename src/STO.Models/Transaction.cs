namespace STO.Models
{
    public class Transaction
    {
        public Transaction(TransactionEntity transactionEntity)
        {
            this.TransactionEntity = transactionEntity;
        } 
        public TransactionEntity TransactionEntity { get; set; } = default!;
        
        public Player Player { get; set; } = default!;
    }
}