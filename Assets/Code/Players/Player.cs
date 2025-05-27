using Code.VisionCone;
using UnityEngine;

namespace Code.Players
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private GameObject _parentVisionMesh;
        
        private IVisionMeshGenerator _visionMeshGenerator;
        
        public void SetVisionMeshGenerator(IVisionMeshGenerator visionMeshGenerator)
        {
            _visionMeshGenerator = visionMeshGenerator;
        }
    }    
}