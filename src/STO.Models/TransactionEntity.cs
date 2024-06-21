using System.ComponentModel.DataAnnotations;
using Azure;
using Azure.Data.Tables;

namespace STO.Models
{
    public class TransactionEntity : ITableEntity
    {
        [Required]
        public string PlayerRowKey { get; set; } = default!;
        [Required]
        public double Amount { get; set; }
        [Required]
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow!;
        public string Notes { get; set; } = default!;
        public string UrlSegment { get; set; }
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow!;
        public ETag ETag { get; set; } = default!;
    }
}