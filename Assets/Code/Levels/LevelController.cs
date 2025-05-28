using System.Collections.Generic;
using Code.Enemies.Provider;
using UnityEngine;
using Code.Players;
using Code.Players.Provider;
using Code.VisionCone;
using Code.VisionCone.Factory;
using DG.Tweening;

namespace Code.Levels
{
    public class LevelController : ILevelController
    {
        private readonly IPlayerProvider _playerProvider;
        private readonly IEnemyProvider _enemiesProvider;
        private readonly IVisionConeFactory _visionConeFactory;
        
        private LevelPathMover _levelPathMover;

        public LevelController(
            IPlayerProvider playerProvider,
            IEnemyProvider enemiesProvider,
            IVisionConeFactory visionConeFactory)
        {
            _playerProvider = playerProvider;
            _enemiesProvider = enemiesProvider;
            _visionConeFactory = visionConeFactory;
        }
        
        public void SetLevelPathMover(LevelPathMover levelPathMover)
        {
            _levelPathMover = levelPathMover;
        }
        
        public void OnPlayLevel(VisionType visionType, float timeLevel)
        {
            SetVisionConeToPlayer(visionType);

            PlayAnimationMovePlayer(timeLevel);
        }

        private void SetVisionConeToPlayer(VisionType visionType)
        {
            GameObject playerPosition = _playerProvider.Player.ParentVisionMesh;
            IVisionMeshGenerator visionCone = _visionConeFactory.CreateVisionMesh(playerPosition, visionType);
            _playerProvider.Player.SetBaseVisionMesh(visionCone);
        }
        
        private void PlayAnimationMovePlayer(float timeLevel)
        {
            Player.transform.DOPath(GetPath(), timeLevel,
                    PathType.CatmullRom,
                    PathMode.Full3D,
                    10,
                    Color.green)
                .SetOptions(true)
                .SetEase(Ease.Linear)
                .OnComplete(() => Debug.Log("Level Completed!"));
        }
        
        private Vector3[] GetPath()
        {
            List<Vector3> path = _levelPathMover.Points;
            path.Reverse();
            return path.ToArray();
        }
        
        private Player Player => _playerProvider.Player;
    }
}