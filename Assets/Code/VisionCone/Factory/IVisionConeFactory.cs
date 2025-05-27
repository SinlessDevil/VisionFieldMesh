using UnityEngine;

namespace Code.VisionCone.Factory
{
    public interface IVisionConeFactory
    {
        IVisionMeshGenerator CreateVisionMesh(GameObject parent, VisionType visionType);
    }
}