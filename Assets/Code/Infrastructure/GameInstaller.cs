using System;
using System.Collections.Generic;
using Code.Infrastructure.Services.GameStater;
using Code.Levels;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure
{
    public class GameInstaller : MonoInstaller, IInitializable, IDisposable
    {
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private List<Transform> _enemSpawnPoints;
        [SerializeField] private LevelController _levelViewController;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameInstaller>().FromInstance(this).AsSingle();
            Container.Bind<IGameStarter>().To<GameStarter>().AsSingle();
        }

        public void Initialize()
        {
            Container.Resolve<IGameStarter>().SetPlayerSpawnPoint(_playerSpawnPoint);
            Container.Resolve<IGameStarter>().SetEnemySpawnPoints(_enemSpawnPoints);
            Container.Resolve<IGameStarter>().SetLevelController(_levelViewController);
            Container.Resolve<IGameStarter>().Initialize();
        }

        public void Dispose()
        {
            Container.Resolve<IGameStarter>().Dispose();
        }
    }
}