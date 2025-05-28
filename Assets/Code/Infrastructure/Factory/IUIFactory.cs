using Code.UI.Hud;
using UnityEngine;

namespace Code.Infrastructure.Factory
{
    public interface IUIFactory
    {
        Canvas UIRootCanvas { get; }
        GameHud GameHud { get; }
        void CreateUIRoot();
        GameHud CreateGameHud();
    }
}