using System.Collections.Generic;
using System.Linq;
using Code.Cameras.Provider;
using Code.Enemies.Factory;
using Code.Enemies.Provider;
using Code.Infrastructure.Factory;
using Code.Infrastructure.Services.PersistenceProgress;
using Code.Infrastructure.Services.PersistenceProgress.Player;
using Code.Infrastructure.Services.SaveLoad;
using Code.Levels;
using Code.Players;
using Code.Players.Factory;
using Code.Players.Provider;
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
        private readonly ICameraProvider _cameraProvider;
        private readonly IPlayerProvider _playerProvider;
        private readonly IEnemyProvider _enemiesProvider;
        
        private Transform _playerSpawnPoint;
        private List<Transform> _enemySpawnPoints;
        private LevelPathMover _levelPathMover;

        public GameStarter(
            IPersistenceProgressService progressService,
            ISaveLoadService saveLoadService, 
            IUIFactory uiFactory, 
            IPlayerFactory playerFactory, 
            IEnemyFactory enemyFactory, 
            ICameraProvider cameraProvider, 
            IPlayerProvider playerProvider, 
            IEnemyProvider enemiesProvider)
        {
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            
            _uiFactory = uiFactory;
            _playerFactory = playerFactory;
            _enemyFactory = enemyFactory;
            
            _cameraProvider = cameraProvider;
            _playerProvider = playerProvider;
            _enemiesProvider = enemiesProvider;
        }

        public void Initialize()
        {
            Debug.Log("GameStarter.Initialize");
            
            InitProgress();
            InitUI();
            InitCamera();
            InitLevel();
        }

        public void SetPlayerSpawnPoint(Transform playerSpawnPoint) => _playerSpawnPoint = playerSpawnPoint;

        public void SetEnemySpawnPoints(List<Transform> enemySpawnPoints) => _enemySpawnPoints = enemySpawnPoints;

        public void SetLevelPathMover(LevelPathMover levelPathMover) => _levelPathMover = levelPathMover;

        public void Dispose()
        {
            
            
            _playerSpawnPoint = null;
            _enemySpawnPoints = null;
            _levelPathMover = null;
            
            _playerProvider.Dispose();
            _enemiesProvider.Dispose();
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
            var player = InitPlayer();
            var enemies = InitEnemies();
            
            Debug.Log("InitLevel");
        }

        private List<Enemy> InitEnemies()
        {
            List<Enemy> enemies = _enemySpawnPoints
                .Select(enemySpawnPoint => _enemyFactory
                    .CreateEnemy(enemySpawnPoint.position))
                .ToList();
            _enemiesProvider.SetEnemies(enemies);
            return enemies;
        }

        private Player InitPlayer()
        {
            Player player = _playerFactory.CreatePlayer(_playerSpawnPoint.position, new VisionArrowMesh());
            _playerProvider.SetPlayer(player);
            return player;
        }

        private void InitCamera()
        {
            _cameraProvider.SetMainCamera(Camera.main);
        }
    }
}