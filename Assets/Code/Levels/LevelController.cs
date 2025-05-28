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
            GameObject playerPosition = _player.ParentVisionMesh;
            IVisionMeshGenerator visionCone = _visionConeFactory.CreateVisionMesh(playerPosition, _visionType);
            _player.SetBaseVisionMesh(visionCone);
            
            _player.transform.DOPath(_levelPathMover.Points.ToArray(), _timeLevel,
                    PathType.CatmullRom,
                    PathMode.Full3D,
                    10,
                    Color.green)
                .SetOptions(true)
                .SetEase(Ease.Linear)
                .OnComplete(() => Debug.Log("Level Completed!"));

        }
    }
}