using Code.UI.Levels;
using UnityEngine;

namespace Code.UI.Hud
{
    public class GameHud : MonoBehaviour
    {
        [SerializeField] private LevelView _levelView;
        
        public LevelView LevelView => _levelView;
    }
}