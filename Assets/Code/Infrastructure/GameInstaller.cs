using System;
using System.Collections.Generic;
using Code.Infrastructure.Services.GameStater;
using Code.Infrastructure.Services.GameUpdapter;
using Code.Levels;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure
{
    public class GameInstaller : MonoInstaller, IInitializable, IDisposable
    {
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private List<Transform> _enemSpawnPoints;
        [SerializeField] private LevelPathMover _levelPathMover;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameInstaller>().FromInstance(this).AsSingle();
            Container.Bind<IGameStarter>().To<GameStarter>().AsSingle();
            Container.Bind<IGameUpdater>().To<GameUpdater>().AsSingle();
        }

        public void Initialize()
        {
            Container.Resolve<IGameStarter>().SetPlayerSpawnPoint(_playerSpawnPoint);
            Container.Resolve<IGameStarter>().SetEnemySpawnPoints(_enemSpawnPoints);
            Container.Resolve<IGameStarter>().SetLevelPathMover(_levelPathMover);
            Container.Resolve<IGameStarter>().Initialize();
            
            Container.Resolve<IGameUpdater>().Initialize();
        }

        public void Update()
        {
            Container.Resolve<IGameUpdater>().Update();
        }

        public void Dispose()
        {
            Container.Resolve<IGameStarter>().Dispose();
        }
    }
}