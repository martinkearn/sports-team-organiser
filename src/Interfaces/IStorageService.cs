using STO.Models;

namespace STO.Interfaces
{
    /// <summary>
    /// Service for working with Azure Storage.
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Adds or Updates a player. Updates if rowkey is present, adds if not.
        /// </summary>
        /// <param name="player">The player to upsert.</param>
        /// <returns>Stored representation of the player.</returns>
        public Task<Player> UpsertPlayer(Player player);

        /// <summary>
        /// Deletes a player.
        /// </summary>
        /// <param name="rowKey">The rowKey value for the player to delete.</param>
        /// <returns>Nothing.</returns>
        public void DeletePlayer(string rowKey);

        /// <summary>
        /// Queries players usiong an Odata query syntax.
        /// </summary>
        /// <param name="player">The Odata filter string.</param>
        /// <returns>A list of players which match the query.</returns>
        public List<Player> QueryPlayers(string filter);

        /// <summary>
        /// Returns configuration data.
        /// </summary>
        /// <returns>StorageConfiguration based on current environment.</returns>
        public StorageConfiguration GetConfiguration();
    }
}