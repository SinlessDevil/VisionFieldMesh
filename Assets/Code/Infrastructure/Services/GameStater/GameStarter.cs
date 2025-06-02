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
using Code.UI.Levels;
using Code.VisionCone;
using Code.VisionCone.Visions;
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
        private readonly ILevelController _levelController;
        
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
            IEnemyProvider enemiesProvider, 
            ILevelController levelController)
        {
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            
            _uiFactory = uiFactory;
            _playerFactory = playerFactory;
            _enemyFactory = enemyFactory;
            
            _cameraProvider = cameraProvider;
            _playerProvider = playerProvider;
            _enemiesProvider = enemiesProvider;
            _levelController = levelController;
        }

        public void Initialize()
        {
            Debug.Log("GameStarter.Initialize");
            
            InitProgress();
            InitUI();
            InitCamera();
            InitLevel();
            InitGameHud();
        }

        private void InitGameHud()
        {
            _uiFactory.CreateGameHud();
            _uiFactory.GameHud.Initialize(InitLevelPresenter());
        }

        private LevelPresenter InitLevelPresenter()
        {
            var levelPresenter = new LevelPresenter(_levelController);
            return levelPresenter;
        }
        
        public void SetPlayerSpawnPoint(Transform playerSpawnPoint) => _playerSpawnPoint = playerSpawnPoint;

        public void SetEnemySpawnPoints(List<Transform> enemySpawnPoints) => _enemySpawnPoints = enemySpawnPoints;

        public void SetLevelPathMover(LevelPathMover levelPathMover) => _levelPathMover = levelPathMover;

        public void Dispose()
        {
            _uiFactory.GameHud.Dispose();
            
            _playerSpawnPoint = null;
            _enemySpawnPoints = null;
            _levelPathMover = null;
            
            _levelController.Dispose();
            
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
            InitPlayer();
            InitEnemies();
            InitLevelController();
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

        private void InitLevelController()
        {
            _levelController.SetLevelPathMover(_levelPathMover);
        }
        
        private void InitCamera()
        {
            _cameraProvider.SetMainCamera(Camera.main);
        }
    }
}