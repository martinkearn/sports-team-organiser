using System.ComponentModel.DataAnnotations;
using Azure;
using Azure.Data.Tables;

namespace STO.Server.Models
{
    public class RatingEntity : ITableEntity
    {
        [Required]
        public string PlayerRowKey { get; set; } = default!;
        [Required]
        public string GameRowKey { get; set; } = default!;
        [Required]
        public double Rating { get; set; } = 0;
        public string Notes { get; set; } = default!;
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow!;
        public ETag ETag { get; set; } = default!;
    }
}