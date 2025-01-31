namespace STO.Models;

public class Game
{
    private DateTime _dateTime;
    
    public string Id { get; set; } = null!;
    
    [Required]
    public DateTime DateTime
    {
        get => _dateTime;
        set
        {
            _dateTime = value;
            UrlSegment = value.Date.ToString("dd-MM-yyyy");
        }
    }
    
    public string Label { get; set; } = null!;
    
    public int TeamAGoals { get; set; }

    public int TeamBGoals { get; set; }
    
    public string Notes { get; set; }
    
    /// <summary>
    /// Format dd-MM-yyyy
    /// </summary>
    public string UrlSegment { get; set; }
}