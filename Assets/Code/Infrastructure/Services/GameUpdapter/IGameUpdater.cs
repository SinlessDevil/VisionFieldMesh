namespace Code.Infrastructure.Services.GameUpdapter
{
    public interface IGameUpdater
    {
        void Initialize();
        void Update();
        void Dispose();
    }
}