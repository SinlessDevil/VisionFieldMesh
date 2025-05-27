using Zenject;
using Code.VisionCone;
using UnityEngine;

namespace Code.Players.Factory
{
    public class PlayerFactory : Infrastructure.Factory.Factory, IPlayerFactory
    {
        private const string PlayerPrefabPath = "Player/Player";
        
        public PlayerFactory(IInstantiator instantiator) : base(instantiator)
        {
            
        }

        public Player CreatePlayer(Vector3 position, IVisionMeshGenerator visionMeshGenerator)
        {
            var gameObject = Instantiate(PlayerPrefabPath, position, Quaternion.identity, null);
            var player = gameObject.GetComponent<Player>();
            player.transform.position = position;
            player.SetVisionMeshGenerator(visionMeshGenerator);
            return player;
        }
    }   
}