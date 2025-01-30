namespace STO.Wasm.Interfaces;

public interface IPlayerAtGameEntityService
{
    /// <summary>
    /// Gets a list of PlayerAtGame for a given Game
    /// </summary>
    /// <param name="gameRowKey">The RowKey for the GameEntity to delete.</param>
    /// <returns>List of PlayerAtGame.</returns>
    public List<PlayerAtGameEntity> GetPlayerAtGameEntitiesForGame(string gameRowKey);

    /// <summary>
    /// Gets a single Pag (player at game).
    /// </summary>
    /// <param name="rowKey">The RowKey for the GameEntity to get.</param>
    /// <returns>PlayerAtGame.</returns>
    public PlayerAtGameEntity GetPlayerAtGameEntity(string rowKey);
		
    /// <summary>
    /// Gets a all PlayerAtGame entities
    /// </summary>
    /// <returns>List of PlayerAtGameEntity.</returns>
    public List<PlayerAtGameEntity> GetPlayerAtGameEntities();	
    
    /// <summary>
    /// Gets a single Pag (player at game).
    /// </summary>
    /// <param name="urlSegment">The UrlSegment for the PlayerAtGameEntity to get.</param>
    /// <returns>PlayerAtGame.</returns>
    public PlayerAtGameEntity GetPlayerAtGameEntityByUrlSegment(string urlSegment);

    /// <summary>
    /// Upserts a PlayerAtGameEntity.
    /// </summary>
    /// <param name="pagEntity">The PlayerAtGame.</param>     
    public Task UpsertPlayerAtGameEntityAsync(PlayerAtGameEntity pagEntity);

    /// <summary>
    /// Deletes a PlayerAtGameEntity from a game.
    /// </summary>
    /// <param name="rowKey">The RowKey for the PlayerAtGame to delete.</param>     
    public Task DeletePlayerAtGameEntityAsync(string rowKey);
    
    /// <summary>
    /// Distributes PlayerAtGames into teams
    /// </summary>
    /// <param name="pags">The PlayerAtGame collection to assign teams to.</param> 
    /// <returns>List of PlayerAtGame with team details.</returns>   
    public Task<List<PlayerAtGameEntity>> CalculateTeamsAsync(List<PlayerAtGameEntity> pags);

    /// <summary>
    /// Removes the team assignment for all PlayerAtGame in a Game
    /// </summary>
    /// <param name="gameRowKey">The RowKey for the GameEntity to remove team asisgnments for</param>
    /// <returns>Nothing</returns>
    public Task ResetTeamsAsync(string gameRowKey);
    
    /// <summary>
    /// Toggles whether a player has played at a game or not and creates/removes trasnactions.
    /// </summary>
    /// <param name="pag">The PlayerAtGame to toggle the Played property for. Not used if null.</param>
    /// <param name="played">Indicates whether to mark the PlayerAtGameEntity as Played or not</param>  
    public Task TogglePlayerAtGamePlayedAsync(PlayerAtGameEntity pag, bool? played);
    
    /// <summary>
    /// Returns a label for the PlayerAtGameEntity based on the player name and game's label
    /// </summary>
    /// <param name="rowKey">The RowKey of the PlayerAtGameEntity to create a label for.</param>
    /// <param name="length">Should the label be short or long.</param>
    /// <returns>A string which represents the label for the PlayerAtGameEntity.</returns>
    public string GetPlayerAtGameLabel(string rowKey, Enums.TitleLength length = Enums.TitleLength.Short);
    
    /// <summary>
    /// Returns the most recent PlayerAtGameEntity to be added to a game
    /// </summary>
    /// <param name="rowKey">The RowKey of the GameEntity to get the most recent PlayerAtGameEntity for.</param>
    /// <returns>Most recent PlayerAtGameEntity in a GameEntity</returns>
    public PlayerAtGameEntity? GetMostRecentPlayerAtGameForGame(string rowKey);
}