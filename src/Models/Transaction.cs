using System.ComponentModel.DataAnnotations;

namespace STO.Models
{
    public class Transaction
    {
        [Required]
        public string Id { get; set; } = default!;
        [Required]
        public string PlayerRowKey { get; set; } = default!;
        [Required]
        public double Amount { get; set; }
        [Required]
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow!;
        public string Notes { get; set; } = default!;
        public string GameRowKey { get; set; } = default!;
    }
}