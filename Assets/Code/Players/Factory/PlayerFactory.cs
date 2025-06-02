using Zenject;
using Code.VisionCone;
using Code.VisionCone.Visions;
using UnityEngine;

namespace Code.Players.Factory
{
    public class PlayerFactory : Infrastructure.Factory.Factory, IPlayerFactory
    {
        private const string PlayerPrefabPath = "Player/Player";
        
        public PlayerFactory(IInstantiator instantiator) : base(instantiator)
        {
            
        }

        public Player CreatePlayer(Vector3 position, BaseVisionMesh baseVisionMesh)
        {
            var gameObject = Instantiate(PlayerPrefabPath, position, Quaternion.identity, null);
            var player = gameObject.GetComponent<Player>();
            player.transform.position = position;
            player.SetBaseTargetDetector(baseVisionMesh);
            return player;
        }
    }   
}