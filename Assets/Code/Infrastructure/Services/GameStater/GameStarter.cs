using System.Collections.Generic;
using System.Linq;
using Code.Enemies.Factory;
using Code.Infrastructure.Factory;
using Code.Infrastructure.Services.PersistenceProgress;
using Code.Infrastructure.Services.PersistenceProgress.Player;
using Code.Infrastructure.Services.SaveLoad;
using Code.Players;
using Code.Players.Factory;
using Code.VisionCone;
using UnityEngine;

namespace Code.Infrastructure.Services.GameStater
{
    public class GameStarter : IGameStarter
    {
        private readonly IPersistenceProgressService _progressService;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IUIFactory _uiFactory;
        private readonly IPlayerFactory _playerFactory;
        private readonly IEnemyFactory _enemyFactory;
        
        private Transform _playerSpawnPoint;
        private List<Transform> _enemySpawnPoints;

        public GameStarter(
            IPersistenceProgressService progressService,
            ISaveLoadService saveLoadService, 
            IUIFactory uiFactory, 
            IPlayerFactory playerFactory)
        {
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            _uiFactory = uiFactory;
            _playerFactory = playerFactory;
        }

        public void Initialize()
        {
            Debug.Log("GameStarter.Initialize");
            
            InitProgress();
            InitUI();
            InitLevel();
        }

        public void SetPlayerSpawnPoint(Transform playerSpawnPoint)
        {
            _playerSpawnPoint = playerSpawnPoint;
        }

        public void SetEnemySpawnPoints(List<Transform> enemySpawnPoints)
        {
            _enemySpawnPoints = enemySpawnPoints;
        }

        public void Dispose()
        {
            _playerSpawnPoint = null;
            _enemySpawnPoints = null;
        }
        
        private void InitProgress()
        {
            _progressService.PlayerData = LoadProgress() ?? SetUpBaseProgress();   
        }
        
        private void InitUI()
        {
            _uiFactory.CreateUIRoot();
            _uiFactory.CreateGameHud();
        }
        
        private PlayerData LoadProgress()
        {
            Debug.Log("LoadProgress");

            return _saveLoadService.Load();
        }

        private PlayerData SetUpBaseProgress()
        {
            Debug.Log("InitializeProgress");
            
            var progress = new PlayerData();
            _progressService.PlayerData = progress;
            return progress;
        }
        
        private void InitLevel()
        {
            Debug.Log("InitLevel");
            Player player = _playerFactory.CreatePlayer(_playerSpawnPoint.position, new VisionArrowMesh());

            List<Enemy> enemies = _enemySpawnPoints
                .Select(enemySpawnPoint => _enemyFactory
                    .CreateEnemy(enemySpawnPoint.position))
                .ToList();
        }
    }
}