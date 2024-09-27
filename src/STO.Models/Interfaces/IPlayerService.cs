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
    /// Gets all Player objects which have played within a given timeframe.
    /// </summary>
    /// <param name="dateRangeStart">DateTime for the start of the date range to return Players from.</param>
    /// <param name="dateRangeEnd">DateTime for the end of the date range to return Players from.</param>
    /// <returns>List of Player.</returns>
    public List<Player> GetPlayers(DateTime dateRangeStart, DateTime dateRangeEnd);
    
    /// <summary>
    /// Gets a Player based on Id
    /// </summary>
    /// <param name="id">The Id to match on</param>
    /// <returns>A Player.</returns>
    public Player GetPlayer(string id);
    
    /// <summary>
    /// Deletes the Player, Transaction(s) and PlayerAtGame(s) associated with a Player.
    /// </summary>
    /// <param name="id">The Id for the Player to delete.</param>
    public Task DeletePlayerAsync(string id);

    /// <summary>
    /// Adds a new Player or updates and existing Player.
    /// </summary>
    /// <param name="player">The Player to upsert.</param>
    public Task UpsertPlayerAsync(Player player);
}