namespace STO.Models.Interfaces;

public interface IPlayerAtGameService
{
    /// <summary>
    /// Gets all PlayerAtGame.
    /// </summary>
    /// <param name="skip">How many items to skip before taking. For example, if set to 30, the return will start from the 31st item. If null, no items will be skipped</param>
    /// <param name="take">How many items to take For example, if set to 20, 20 items will be returned. If null, all items will be taken from the skip</param>
    /// <returns>List of PlayerAtGame.</returns>
    public List<PlayerAtGame> GetPags(int? skip, int? take);

    /// <summary>
    /// Gets all PlayerAtGame for a specified Game
    /// </summary>
    /// <param name="gameId">The Game ID to get PlayerAtGame for.</param>
    /// <returns>List of PlayerAtGame.</returns>
    public List<PlayerAtGame> GetPagsForGame(string gameId);
    
    /// <summary>
    /// Organises a list of PlayerAtGame into fair and equal teams based on position and rating. Assigns a team to each PlayerAtGame.
    /// </summary>
    /// <param name="pags">The list of PlayerAtGame to organise into teams.</param>
    /// <returns>List of PlayerAtGame.</returns>
    public Task<List<PlayerAtGame>> OrganiseTeams(List<PlayerAtGame> pags);
    
    /// <summary>
    /// Removes team association for each PlayerAtGame in given list.
    /// </summary>
    /// <param name="pags">List of PlayerAtGame to remove team association for.</param>
    /// <returns>Nothing.</returns>
    public Task ResetTeamsAsync(List<PlayerAtGame> pags);

    /// <summary>
    /// Toggles the Played property of the specified PlayerAtGame. Adds or removes Transactions in accordance with new Played status
    /// </summary>
    /// <param name="pagId">The ID for the PlayerAtGame to toggle</param>
    /// <returns>Nothing.</returns>
    public Task TogglePagPlayedAsync(string pagId);
    
    /// <summary>
    /// Gets a single PlayerAtGame by Id.
    /// </summary>
    /// <param name="id">The Id for the PlayerAtGame to get.</param>
    /// <returns>PlayerAtGame.</returns>
    public PlayerAtGame GetPag(string id);
    
    /// <summary>
    /// Gets a single PlayerAtGame by UrlSegment.
    /// </summary>
    /// <param name="urlSegment">The UrlSegment for the PlayerAtGame to get.</param>
    /// <returns>PlayerAtGame.</returns>
    public PlayerAtGame GetPagByUrlSegment(string urlSegment);
    
    /// <summary>
    /// Deletes the PlayerAtGame.
    /// </summary>
    /// <param name="id">The Id for the PlayerAtGame to delete.</param>
    public Task DeletePagAsync(string id);

    /// <summary>
    /// Adds a new PlayerAtGame or updates if it already exists.
    /// </summary>
    /// <param name="pag">The PlayerAtGame to upsert.</param>
    public Task UpsertPagAsync(PlayerAtGame pag);
}