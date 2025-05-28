using Code.UI.Levels;
using UnityEngine;

namespace Code.UI.Hud
{
    public class GameHud : MonoBehaviour
    {
        [SerializeField] private LevelView _levelView;

        public void Initialize(LevelPresenter presenter)
        {
            _levelView.Initialize(presenter);
        }

        public void Dispose()
        {
            _levelView.Dispose();
        }
    }
}