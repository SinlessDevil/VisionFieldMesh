using Code.VisionCone.Visions;
using UnityEngine;

namespace Code.VisionCone.Factory
{
    public interface IVisionConeFactory
    {
        BaseVisionMesh CreateVisionMesh(GameObject parent, VisionType visionType);
    }
}