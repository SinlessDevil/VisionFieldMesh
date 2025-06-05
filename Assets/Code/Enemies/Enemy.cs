using UnityEngine;

namespace Code
{
    public class Enemy : MonoBehaviour
    {
        private const string IsDetected = "isDetected";
        
        [SerializeField] private Collider _collider;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private Animator _animator;
        
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
        
        public void SetAnimationDetected()
        {
            _animator.SetTrigger(IsDetected);
        }
    }
}