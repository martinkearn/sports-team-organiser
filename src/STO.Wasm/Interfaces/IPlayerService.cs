namespace STO.Wasm.Interfaces;

/// <summary>
/// Service for working with Players. Virtual objects made from PlayerEntity with calculated properties from other entities
/// </summary>
public interface IPlayerService
{
    /// <summary>
    /// Gets a Player from RowKey
    /// </summary>
    /// <param name="rowKey">The RowKey for the PlayerEntity to match on</param>
    /// <returns>A Player.</returns>
    public Player GetPlayer(string rowKey);
}