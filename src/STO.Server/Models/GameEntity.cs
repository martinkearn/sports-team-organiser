using System.ComponentModel.DataAnnotations;
using Azure;
using Azure.Data.Tables;

namespace STO.Server.Models
{
    public class GameEntity : ITableEntity
    {
        [Required]
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow!;

        public int TeamAGoals { get; set; }

        public int TeamBGoals { get; set; }

        public string Title { get; set; }

        public string Notes { get; set; }

        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow!;
        public ETag ETag { get; set; } = default!;
    }
}