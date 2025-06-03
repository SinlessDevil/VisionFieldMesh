using UnityEngine;

namespace Code.Weapon
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _shootEffect;
        [SerializeField] private GameObject _pivot;
        [SerializeField] private GameObject _model;
        
        public GameObject Pivot => _pivot;
        
        public GameObject Model => _model;
        
        public void PlayShootEffect()
        {
            _shootEffect.Play();
        }
    }
}