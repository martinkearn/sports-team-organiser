namespace STO.Models;

public class PlayerAtGame
{
    /// <summary>
    /// Primary key for PlayerAtGame
    /// </summary>
    [Required] public string Id { get; set; } = null!;
    
    /// <summary>
    /// The PLayerAtGame's forecasted playing status 
    /// </summary>
    [Required] public Enums.PlayingStatus Forecast { get; set; }
    
    /// <summary>
    /// Has the PlayerAtGame played at the associated Game or not
    /// </summary>
    [Required] public bool Played { get; set;} = false;
    
    /// <summary>
    /// Which Team the PlayerAtGame is on, A or B
    /// </summary>
    public string Team { get; set; } = null!;
    
    /// <summary>
    /// Url for the PlayerAtGame
    /// </summary>
    public string UrlSegment { get; set; } = null!;

    /// <summary>
    /// The label of the PLayerAtGame to use in titles and lists when PlayerName is not appropriate.
    /// </summary>
    public string Label => $"{PlayerLabel}-at-{GameLabel}";
    
    /// <summary>
    /// Format dd-MM-yyyy
    /// </summary>
    public DateTime LastUpdated { get; set; }
    
    /// <summary>
    /// Primary key for associated Player
    /// </summary>
    [Required] public string PlayerId { get; set; } = null!;
    
    /// <summary>
    /// The Name for the associated Player
    /// </summary>
    public string PlayerName { get; set; } = null!;
    
    /// <summary>
    /// The Position for the associated Player
    /// </summary>
    public Enums.PlayerPosition PlayerPosition { get; set; }
    
    /// <summary>
    /// The DefaultRate for the associated Player
    /// </summary>
    public double PlayerDefaultRate { get; set; }
    
    /// <summary>
    /// The AdminRating for the associated Player
    /// </summary>
    public int PlayerAdminRating { get; set; }

    /// <summary>
    /// The average Rating for the associated PLayer
    /// </summary>
    public double PlayerRating { get; set; }
    
    /// <summary>
    /// The Label for the associated Player
    /// </summary>
    public string PlayerLabel { get; set; } = null!;
    
    /// <summary>
    /// The UrlSegmenmt for the associated Player
    /// </summary>
    public string PlayerUrlSegment { get; set; } = null!;
    
    /// <summary>
    /// The Balance of the associated Player
    /// </summary>
    public double PlayerBalance { get; set; } 
    
    /// <summary>
    /// The number of games played by the associated Player
    /// </summary>
    public int PlayerGamesCount { get; set; }
    
    /// <summary>
    /// Primary key for associated Game
    /// </summary>
    [Required] public string GameId { get; set; } = null!;
    
    /// <summary>
    /// The Date and Time for when the associated Game will start
    /// </summary>
    public DateTime GameDateTime { get; set; }
    
    /// <summary>
    /// The Label for the associated Game
    /// </summary>
    public string GameLabel { get; set; } = null!;
    
    /// <summary>
    /// The UrlSegment for the associated Game
    /// </summary>
    public string GameUrlSegment { get; set; } = null!;
}