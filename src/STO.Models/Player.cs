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
            Label = value;
        }
    }


    public string Tags { get; set; }
    public Enums.PlayerPosition Position { get; set; }
    public double DefaultRate { get; set; }
    public int AdminRating { get; set; }
    
    // Calculated properties
    public string Label { get; set; }
    public int Rating { get; set; }
    public string UrlSegment { get; set; }
    public double Balance { get; set; }
    
}