using Code.Cameras;
using Code.Cameras.Provider;
using Code.Players.Provider;
using UnityEngine;

namespace Code.Infrastructure.Services.GameUpdapter
{
    public class GameUpdater : IGameUpdater
    {
        private readonly ICameraProvider _cameraProvider;
        private readonly IPlayerProvider _playerProvider;

        private CameraFollowing _cameraFollowing;
        
        public GameUpdater(
            ICameraProvider cameraProvider,
            IPlayerProvider playerProvider)
        {
            _cameraProvider = cameraProvider;
            _playerProvider = playerProvider;
        }

        public void Initialize()
        {
            _cameraProvider.SetMainCamera(Camera.main);
            _cameraFollowing = new CameraFollowing(_playerProvider.Player.transform, _cameraProvider);
        }

        public void Update()
        {
            _cameraFollowing.Update();
        }

        public void Dispose()
        {
            _cameraFollowing = null;
        }
    }
}