namespace STO.Models;

/// <summary>
/// A Game.
/// </summary>
public class Game
{
    private DateTime _dateTime;
    
    public string Id { get; set; } = null!;
    
    /// <summary>
    /// The DateTimer for when teh game will start.
    /// </summary>
    [Required]
    public DateTime DateTime
    {
        get => _dateTime;
        set
        {
            _dateTime = value;
            
            // Set UrlSegment
            UrlSegment = value.Date.ToString("dd-MM-yyyy");
        }
    }

    /// <summary>
    /// The label of the game to use in titles and lists.
    /// </summary>
    public string Label => string.IsNullOrEmpty(Title) ? $"{DateTime.Date:dd MMM}": $"{DateTime.Date:dd MMM} {Title}"; 
    
    /// <summary>
    /// The number of goals scored by team A during the game
    /// </summary>
    public int TeamAGoals { get; set; }

    /// <summary>
    /// The number of goals scored by team A during the game
    /// </summary>
    public int TeamBGoals { get; set; }
    
    /// <summary>
    /// The title of the game. Used for special games. Automatically included in Label.
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// Notes for the game.
    /// </summary>
    public string Notes { get; set; }
    
    /// <summary>
    /// Format dd-MM-yyyy
    /// </summary>
    public string UrlSegment { get; set; }
}