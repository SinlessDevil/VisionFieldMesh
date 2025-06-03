using Code.VisionCone.Detectors;
using UnityEngine;

namespace Code.VisionCone
{
    public class VisionAI : MonoBehaviour
    {
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
        }

        private void HandleTargetLost(Enemy enemy)
        {
            Debug.Log($"Enemy missing {enemy.name}");
        }
    }   
}