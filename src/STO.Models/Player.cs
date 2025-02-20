using System.Globalization;

namespace STO.Models;

public class Player
{
    // Private properties
    private string _name;
    
    // Properties from PLayerEntity
    public string Id { get; set; }
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            UrlSegment = value.Replace(" ", "-").ToLowerInvariant();
        }
    }
    public string Tags { get; set; }
    public Enums.PlayerPosition Position { get; set; }
    public double DefaultRate { get; set; }
    public int AdminRating { get; set; }
    public DateTime LastUpdated { get; set; }
    
    // Calculated properties
    public string Label => new CultureInfo("en-GB", false).TextInfo.ToTitleCase(Name);
    public double Rating { get; set; }
    public string UrlSegment { get; set; }
    public double Balance { get; set; }
    public int GamesCount { get; set; } 
}