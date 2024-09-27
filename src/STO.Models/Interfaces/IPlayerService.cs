namespace STO.Models.Interfaces;

/// <summary>
/// Service for working with Players. Virtual objects made from PlayerEntity with calculated properties from other entities
/// </summary>
public interface IPlayerService
{
    /// <summary>
    /// Gets all Player objects.
    /// </summary>
    /// <returns>List of Player.</returns>
    public List<Player> GetPlayers();
    
    /// <summary>
    /// Gets all Player objects for a given Game.
    /// </summary>
    /// <param name="gameId">The Id of the Game to get PLayers for.</param>
    /// <returns>List of Player.</returns>
    public List<Player> GetPlayers(string gameId);
    
    /// <summary>
    /// Gets a Player based on Id
    /// </summary>
    /// <param name="id">The Id to match on</param>
    /// <returns>A Player.</returns>
    public Player GetPlayer(string id);
}