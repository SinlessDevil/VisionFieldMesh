using Code.VisionCone;
using UnityEngine;

namespace Code.Players.Factory
{
    public interface IPlayerFactory
    {
        Player CreatePlayer(Vector3 position, IVisionMeshGenerator visionMeshGenerator);
    }
}