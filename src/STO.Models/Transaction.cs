#nullable enable
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace STO.Models;

public class Transaction()
{
    // Private properties
    private DateTime _dateTime;
    private double _amount;
    private DateTime _lastUpdated = DateTime.UtcNow;
    private string _playerName;
    private string _playerUrlSegment;
    
    // Public events
    public event PropertyChangedEventHandler? PropertyChanged;
    
    // Public properties
    public string Id { get; set; }
    public string PlayerId { get; set; } = null!;
    public string GameId { get; set; } = null!;
    public string GameTitle { get; set; } = null!;
    public string Notes { get; set; } = default!;
    
    public string PlayerName
    {
        get => _playerName;
        set
        {
            // Set this property
            _playerName = value;
            
            // Set dependant calculated properties
            SetCalculatedProperties(Amount, DateTime, value, PlayerUrlSegment);
        }
    }
    public string PlayerUrlSegment
    {
        get => _playerUrlSegment;
        set
        {
            // Set this property
            _playerUrlSegment = value;
            
            // Set dependant calculated properties
            SetCalculatedProperties(Amount, DateTime, PlayerName, value);
        }
    }
    
    public double Amount
    {
        get => _amount;
        set
        {
            // Set this property
            _amount = value;
            
            // Set dependant calculated properties
            SetCalculatedProperties(value, DateTime, PlayerName, PlayerUrlSegment);
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
            SetCalculatedProperties(Amount, value, PlayerName, PlayerUrlSegment);
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

    private void SetCalculatedProperties(double amount, DateTime date, string playerName, string playerUrlSegment)
    {
        UrlSegment = $"{playerUrlSegment}-{amount}-{date:dd-MM-yyyy-HH-mm-ss}";
        Label = $"Transaction by {playerName} for {amount} on {date:dd MMM}";
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