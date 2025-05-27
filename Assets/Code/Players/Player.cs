using Code.VisionCone;
using UnityEngine;

namespace Code.Players
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private GameObject _parentVisionMesh;

        public void SetVisionMeshGenerator(IVisionMeshGenerator visionMeshGenerator)
        {
            
        }
    }    
}