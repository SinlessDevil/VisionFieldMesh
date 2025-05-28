using System;
using Code.VisionCone;
using UnityEngine;

namespace Code.Players
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private GameObject _parentVisionMesh;
        [SerializeField] private PlayerMover _playerMover;
        
        private IVisionMeshGenerator _visionMeshGenerator;

        private void OnValidate()
        {
            if(_playerMover == null)
                _playerMover = GetComponent<PlayerMover>();
        }

        public GameObject ParentVisionMesh => _parentVisionMesh;
        
        public PlayerMover PlayerMover => _playerMover;
        
        public void SetBaseVisionMesh(IVisionMeshGenerator visionMeshGenerator)
        {
            _visionMeshGenerator = visionMeshGenerator;
        }
    }    
}