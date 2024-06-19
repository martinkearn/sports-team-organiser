using System.ComponentModel.DataAnnotations;
using Azure;
using Azure.Data.Tables;

namespace STO.Models
{
    public class PlayerAtGameEntity : ITableEntity
    {
        [Required]
        public string PlayerRowKey { get; set; } = default!;
        [Required]
        public string GameRowKey { get; set; } = default!;
        [Required]
        public string Forecast { get; set; } = "unstated";
        [Required]
        public bool Played { get; set;} = false;
        public string Team { get; set; } = default!;
        public string PartitionKey { get; set; } = default!;

        public string UrlSegment { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow!;
        public ETag ETag { get; set; } = default!;
    }
}