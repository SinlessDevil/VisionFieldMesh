using System.Collections.Generic;
using UnityEngine;
using Code.Players;
using Code.VisionCone;
using Sirenix.OdinInspector;

namespace Code.Levels
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private LevelController _levelController;
     
        private Player _player;
        private List<Enemy> _enemies;
        
        public void Initialize(Player player, List<Enemy> enemies)
        {
            _player = player;
            _enemies = enemies;
        }
        
        [Button]
        public void PlayLevel()
        {
            
        }
    }
}