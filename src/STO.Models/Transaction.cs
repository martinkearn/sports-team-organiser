#nullable enable
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace STO.Models;

public class Transaction(string id, PlayerEntity playerEntity, GameEntity? gameEntity)
{
    // Private properties
    private DateTime _dateTime;
    private double _amount;
    private DateTime _lastUpdated = DateTime.UtcNow;
    
    // Public events
    public event PropertyChangedEventHandler? PropertyChanged;
    
    // Constructed properties
    
    public string Id { get; set; } = id;
    public PlayerEntity PlayerEntity { get; set; } = playerEntity;

    public GameEntity? GameEntity { get; set; } = gameEntity;
    
    // Public properties
    
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
        UrlSegment = $"{PlayerEntity.UrlSegment}-{amount}-{date:dd-MM-yyyy-HH-mm-ss}";
        Label = $"Transaction by {PlayerEntity.Name} for {amount} on {date:dd MMM}";
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