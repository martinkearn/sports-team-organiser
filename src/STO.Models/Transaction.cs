#nullable enable
using System.ComponentModel;
using System.Runtime.CompilerServices;
using STO.Models.Interfaces;

namespace STO.Models;

public class Transaction()
{
    // Private properties
    private DateTime _dateTime;
    private double _amount;
    private DateTime _lastUpdated = DateTime.UtcNow;
    
    public Transaction(IPlayerService playerService, string playerId) : this()
    {
        var thisPlayerService = playerService?? throw new ArgumentNullException(nameof(playerService)); // Null-check for safety
        
        // Use player service to get other player props
        var player = thisPlayerService.GetPlayer(playerId);
        PlayerId = player.Id;
        PlayerName = player.Name;
        PlayerUrlSegment = player.UrlSegment;
    }
    
    // Public events
    public event PropertyChangedEventHandler? PropertyChanged;
    
    // Constructed properties
    public string PlayerId { get; set; } = null!;
    public string PlayerName { get; set; } = null!;
    public string PlayerUrlSegment { get; set; } = null!;
    
    // Properties from TransactionEntity
    public string Id { get; set; }
    public string Notes { get; set; } = default!;
    
    public double Amount
    {
        get => _amount;
        set
        {
            // Set this property
            _amount = value;
            
            // Set dependant calculated properties
            SetCalculatedProperties(value, DateTime);
        }
    }

    public DateTime DateTime
    {
        get => _dateTime;
        set
        {
            // Set this property
            _dateTime = value;
            
            // Set dependant calculated properties
            SetCalculatedProperties(Amount, value);
        }
    }
    
    public DateTime LastUpdated
    {
        get => _lastUpdated;
        private set
        {
            _lastUpdated = value;
            OnPropertyChanged();
        }
    }
    
    // Calculated properties
    public string UrlSegment { get; set; } = default!;
    public string Label { get; set; } = default!;

    private void SetCalculatedProperties(double amount, DateTime date)
    {
        UrlSegment = $"{PlayerUrlSegment}-{amount}-{date:dd-MM-yyyy-HH-mm-ss}";
        Label = $"Transaction by {PlayerName} for {amount} on {date:dd MMM}";
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