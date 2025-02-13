#nullable enable
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace STO.Models;

[ExcludeFromCodeCoverage]
public class Transaction
{
    public string Id { get; set; } = null!;
    public double Amount { get; set; }
    public string Notes { get; set; } = null!;
    public DateTime DateTime { get; set; }
    public string UrlSegment { get; set; } = null!;
    public string Label { get; set; } = null!;
    public string PlayerId { get; set; } = null!;
    public string PlayerName { get; set; } = null!;
    public string PlayerUrlSegment { get; set; } = null!;
    public string GameId { get; set; } = null!;
    public string GameLabel { get; set; } = null!;
    public string GameUrlSegment { get; set; } = null!;
    public DateTime LastUpdated { get; set; }

}