namespace Code.Players.Provider
{
    public interface IPlayerProvider
    {
        Player Player { get; }
        void SetPlayer(Player player);
        void Dispose();
    }
}