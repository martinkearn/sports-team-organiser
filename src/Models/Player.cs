using System.ComponentModel.DataAnnotations;

namespace STO.Models
{
    public class Player
    {
        [Required]
        public string Id { get; set; } = default!;
        [Required]
        public string Name { get; set; }
        public string Tags { get; set; }
        public PlayerPosition Position { get; set; }
        public double DefaultRate { get; set; } = default!;
        public double Balance { get; set; } = default!;
        public List<Transaction> Transactions { get; set; }
    }
}