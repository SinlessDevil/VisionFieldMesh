using Code.VisionCone;
using UnityEngine;

namespace Code.Players
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private GameObject _parentVisionMesh;
        
        private IVisionMeshGenerator _visionMeshGenerator;
        
        public GameObject ParentVisionMesh => _parentVisionMesh;
        
        public void SetBaseVisionMesh(IVisionMeshGenerator visionMeshGenerator)
        {
            _visionMeshGenerator = visionMeshGenerator;
        }
    }    
}