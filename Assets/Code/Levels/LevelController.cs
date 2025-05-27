using System.Collections.Generic;
using UnityEngine;
using Code.Players;
using Code.VisionCone;
using Code.VisionCone.Factory;
using Sirenix.OdinInspector;

namespace Code.Levels
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private LevelPathMover _levelPathMover;
        [Space(10)]
        [SerializeField] private VisionType _visionType;
        
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
            
        }
    }
}