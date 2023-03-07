using Azure;
using Azure.Data.Tables;

namespace STO.Models
{
    public class PlayerAtGame : ITableEntity
    {
        public string PlayerRowKey { get; set; } = default!;
        public string GameRowKey { get; set; } = default!;
        public string Forecast { get; set; } = "unstated";
        public  bool Played { get; set;} = false;
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow!;
        public ETag ETag { get; set; } = default!;
    }
}