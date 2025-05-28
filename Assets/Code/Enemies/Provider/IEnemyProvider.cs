using System.Collections.Generic;

namespace Code.Enemies.Provider
{
    public interface IEnemyProvider
    {
        List<Enemy> Enemies { get; }
        void SetEnemies(List<Enemy> enemies);
        void Dispose();
    }
}