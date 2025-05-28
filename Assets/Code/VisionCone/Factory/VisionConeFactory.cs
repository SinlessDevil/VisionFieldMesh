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
        
        public IVisionMeshGenerator CreateVisionMesh(GameObject parent, VisionType visionType)
        {
            string path = VisionConePrefabsPath + visionType;
            GameObject gameObject = Instantiate(path, parent.transform.position, Quaternion.identity, parent.transform);
            IVisionMeshGenerator visionCone = gameObject.GetComponent<IVisionMeshGenerator>();
            return visionCone;
        }
    }
}