using System.Collections.Generic;
using Code.Players;
using Code.VisionCone;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Levels
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private List<Enemy> _enemies;
        [SerializeField] private List<Transform> _pathPoints;
        
        [Button]
        public void PlayLevel()
        {
            
        }

        private BaseVisionMesh GetBaseVisionMeshByType(VisionType visionType)
        {
            return visionType switch
            {
                VisionType.Circle => gameObject.AddComponent<VisionCircleMesh>(),
                VisionType.Square => gameObject.AddComponent<VisionSquareMesh>(),
                VisionType.OffsetTriangle => gameObject.AddComponent<VisionOffsetTriangleMesh>(),
                VisionType.HalfEllipse => gameObject.AddComponent<VisionHalfEllipseMesh>(),
                VisionType.Arrow => gameObject.AddComponent<VisionArrowMesh>(),
                VisionType.Rhombus => gameObject.AddComponent<VisionRhombusMesh>(),
                _ => null
            };
        }
    }

    public enum VisionType
    {
        Circle = 0,
        Square = 1,
        OffsetTriangle = 2,
        HalfEllipse = 3,
        Arrow = 4,
        Rhombus = 5,
    }
}