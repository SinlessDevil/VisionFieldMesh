using UnityEngine;

namespace Code
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private ParticleSystem _particleSystem;
        
        public void SetRevive()
        {
            _particleSystem.Stop();
            _collider.enabled = true;
            _meshRenderer.enabled = true;
        }
        
        public void SetDead()
        {
            _particleSystem.Play();
            _collider.enabled = false;
            _meshRenderer.enabled = false;
        }
    }
}