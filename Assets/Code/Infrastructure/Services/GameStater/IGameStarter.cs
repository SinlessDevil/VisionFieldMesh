using System.Collections.Generic;
using UnityEngine;

namespace Code.Infrastructure.Services.GameStater
{
    public interface IGameStarter
    {
        void Initialize();
        void SetPlayerSpawnPoint(Transform playerSpawnPoint);
        void SetEnemySpawnPoints(List<Transform> enemySpawnPoints);
        void Dispose();
    }
}