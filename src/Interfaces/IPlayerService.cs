namespace STO.Interfaces
{
    /// <summary>
    /// Service for working with Players.
    /// </summary>
    public interface IPlayerService
    {
        public List<Player> QueryPlayers(string filter);
        public void AddPlayer(string id);
        public void DeletePlayer(string id);
    }
}