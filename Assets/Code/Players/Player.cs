using System;
using Code.VisionCone;
using Code.VisionCone.Detectors;
using Code.VisionCone.Visions;
using UnityEngine;

namespace Code.Players
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private GameObject _parentVisionMesh;
        [SerializeField] private PlayerMover _playerMover;
        [SerializeField] private VisionAI _visionAI;

        private void OnValidate()
        {
            if(_playerMover == null)
                _playerMover = GetComponent<PlayerMover>();
            
            if(_visionAI == null)
                _visionAI = GetComponent<VisionAI>();
        }

        public GameObject ParentVisionMesh => _parentVisionMesh;
        
        public PlayerMover PlayerMover => _playerMover;
        
        public void SetBaseTargetDetector(BaseTargetDetector baseTargetDetector)
        {
            _visionAI.Initialize(baseTargetDetector);
        }

        public void Dispose()
        {
            _visionAI.Dispose();
        }
    }    
}