namespace STO.Models.Interfaces;

public interface IGameService
{
    /// <summary>
    /// Gets all Games.
    /// </summary>
    /// <returns>List of Game.</returns>
    public List<Game> GetGames();
    
    /// <summary>
    /// Gets a single Game by Id.
    /// </summary>
    /// <param name="id">The Id for the Game to get.</param>
    /// <returns>Game.</returns>
    public Game GetGame(string id);
    
    /// <summary>
    /// Gets a single Game by UrlSegment.
    /// </summary>
    /// <param name="urlSegment">The UrlSegment for the Game to get.</param>
    /// <returns>Game.</returns>
    public Game GetGameByUrlSegment(string urlSegment);
    
    /// <summary>
    /// Gets the next Game in terms of date.
    /// </summary>
    /// <returns>Game.</returns>
    public Game GetNextGame();

    /// <summary>
    /// Deletes the Game and PlayerAtGameEntity associated with a Game.
    /// </summary>
    /// <param name="id">The Id for the Game to delete.</param>
    public Task DeleteGameAsync(string id);

    /// <summary>
    /// Adds a new Game.
    /// </summary>
    /// <param name="game">The Game to upsert.</param>
    public Task UpsertGameAsync(Game game);
}