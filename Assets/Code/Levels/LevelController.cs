using System.Collections.Generic;
using UnityEngine;
using Code.Players;
using Code.VisionCone;
using Code.VisionCone.Factory;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Code.Levels
{
    public class LevelController : MonoBehaviour
    {
        [Header("Level Components")]
        [SerializeField] private LevelPathMover _levelPathMover;
        
        [Space(20)] [Header("Start Game")]
        [SerializeField] private VisionType _visionType;
        [SerializeField] private float _timeLevel = 30f;
        
        private Player _player;
        private List<Enemy> _enemies;
        private IVisionConeFactory _visionConeFactory;

        public void Initialize(
            Player player, 
            List<Enemy> enemies,
            IVisionConeFactory visionConeFactory)
        {
            _player = player;
            _enemies = enemies;
            _visionConeFactory = visionConeFactory;
        }
        
        [Button]
        public void PlayLevel()
        {
            SetVisionConeToPlayer();

            PlayAnimationMovePlayer();
        }

        private void SetVisionConeToPlayer()
        {
            GameObject playerPosition = _player.ParentVisionMesh;
            IVisionMeshGenerator visionCone = _visionConeFactory.CreateVisionMesh(playerPosition, _visionType);
            _player.SetBaseVisionMesh(visionCone);
        }
        
        private void PlayAnimationMovePlayer()
        {
            _player.transform.DOPath(GetPath(), _timeLevel,
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
    }
}