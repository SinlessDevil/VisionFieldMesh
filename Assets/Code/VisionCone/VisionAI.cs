using Code.VisionCone.Detectors;
using Code.Weapon;
using UnityEngine;

namespace Code.VisionCone
{
    public class VisionAI : MonoBehaviour
    {
        [SerializeField] private WeaponAI _weaponAI;
        
        private BaseTargetDetector _detector;

        public void Initialize(BaseTargetDetector detector)
        {
            _detector = detector;
            
            _detector.OnTargetDetected += HandleTargetDetected;
            _detector.OnTargetLost += HandleTargetLost;
        }

        public void Dispose()
        {
            _detector.OnTargetDetected -= HandleTargetDetected;
            _detector.OnTargetLost -= HandleTargetLost;
            
            _detector = null;
        }

        private void HandleTargetDetected(Enemy enemy)
        {
            Debug.Log($"Enemy detected {enemy.name}");
            
            _weaponAI.StartFiring(enemy);
        }

        private void HandleTargetLost(Enemy enemy)
        {
            Debug.Log($"Enemy missing {enemy.name}");
            
            _weaponAI.StopFiring(enemy);
        }
    }   
}