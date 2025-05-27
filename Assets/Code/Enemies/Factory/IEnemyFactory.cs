using UnityEngine;

namespace Code.Enemies.Factory
{
    public interface IEnemyFactory
    {
        Enemy CreateEnemy(Vector3 position);
    }
}