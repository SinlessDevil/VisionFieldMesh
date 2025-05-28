using System.Collections.Generic;
using Code.Levels;
using UnityEngine;

namespace Code.Infrastructure.Services.GameStater
{
    public interface IGameStarter
    {
        void Initialize();
        void SetPlayerSpawnPoint(Transform playerSpawnPoint);
        void SetEnemySpawnPoints(List<Transform> enemySpawnPoints);
        void SetLevelPathMover(LevelPathMover levelPathMover);
        void Dispose();
    }
}