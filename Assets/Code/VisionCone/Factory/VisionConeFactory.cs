using Code.VisionCone.Visions;
using UnityEngine;
using Zenject;

namespace Code.VisionCone.Factory
{
    public class VisionConeFactory : Infrastructure.Factory.Factory, IVisionConeFactory
    {
        private string VisionConePrefabsPath => "VisionCone/";
        
        public VisionConeFactory(IInstantiator instantiator) : base(instantiator)
        {
            
        }
        
        public BaseVisionMesh CreateVisionMesh(GameObject parent, VisionType visionType)
        {
            string path = VisionConePrefabsPath + visionType;
            GameObject gameObject = Instantiate(path, parent.transform.position, Quaternion.identity, parent.transform);
            BaseVisionMesh visionCone = gameObject.GetComponent<BaseVisionMesh>();
            return visionCone;
        }
    }
}