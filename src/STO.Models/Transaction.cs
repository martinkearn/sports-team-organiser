#nullable enable
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace STO.Models;

public class Transaction
{
    // Private properties
    private DateTime _lastUpdated = DateTime.UtcNow;
    
    // Public events
    public event PropertyChangedEventHandler? PropertyChanged;
    
    // Public properties
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
    
    public DateTime LastUpdated
    {
        get => _lastUpdated;
        private set
        {
            _lastUpdated = value;
            OnPropertyChanged();
        }
    }

    // This method is called whenever any property changes
    private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // Update LastUpdated unless it is LastUpdated itself
        if (propertyName != nameof(LastUpdated))
        {
            LastUpdated = DateTime.UtcNow;
        }
    }
}