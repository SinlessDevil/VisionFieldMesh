using System.Collections.Generic;

namespace Code.Enemies.Provider
{
    public class EnemyProvider : IEnemyProvider
    {
        public List<Enemy> Enemies { get; private set; }
        
        public void SetEnemies(List<Enemy> enemies)
        {
            Enemies = enemies;
        }
        
        public void Dispose()
        {
            Enemies = null;
        }
    }
}