using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.VisionCone.Detectors
{
    public abstract class BaseTargetDetector : MonoBehaviour
    {
        [BoxGroup("Base Target Detector Settings")] [SerializeField] protected LayerMask _targetMask;
        [BoxGroup("Base Target Detector Settings")] [SerializeField] protected LayerMask _obstacleMask;
        [BoxGroup("Base Target Detector Settings")] [SerializeField] protected Transform _body;
        [BoxGroup("Base Target Detector Settings")] [Space(10)] 
        [BoxGroup("Base Target Detector Settings")] [SerializeField] protected bool _isShowDebug = true;
        [BoxGroup("Base Target Detector Settings")] [SerializeField] protected int _precision = 64;
        
        public Action<Enemy> OnTargetDetected;
        public Action<Enemy> OnTargetLost;
        
        private Enemy _lastTarget;
        
        private void Update()
        {
            Enemy target = FindVisibleTarget();

            if (target != _lastTarget)
            {
                if (_lastTarget != null)
                    OnTargetLost?.Invoke(_lastTarget);

                if (target != null)
                    OnTargetDetected?.Invoke(target);

                _lastTarget = target;
            }
        }

        protected abstract Enemy FindVisibleTarget();
    }
}