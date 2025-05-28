namespace Code.Players.Provider
{
    public class PlayerProvider : IPlayerProvider
    {
        public Player Player { get; private set; }
        
        public void SetPlayer(Player player)
        {
            Player = player;
        }
        
        public void Dispose()
        {
            Player = null;
        }
    }
}