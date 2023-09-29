namespace STO.Models
{
    public class Transaction
    {
        public Transaction(TransactionEntity TransactionEntity)
        {
            this.TransactionEntity = TransactionEntity;
        } 
        public TransactionEntity TransactionEntity { get; set; } = default!;
        
        public Player Player { get; set; } = default!;
    }
}