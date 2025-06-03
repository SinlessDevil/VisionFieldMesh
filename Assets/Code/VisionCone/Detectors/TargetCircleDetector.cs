using System;
using UnityEngine;

namespace Code.VisionCone.Detectors
{
    public class TargetCircleDetector : BaseTargetDetector
    {
        [Header("Detection Settings")] 
        [SerializeField] private float _range = 6f;
        [SerializeField] private float _viewAngle = 360f;
        [SerializeField] private float _heightAbove = 1f;
        [SerializeField] private float _heightBelow = 1f;
        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private Transform _body;

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

        private Enemy FindVisibleTarget()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _range, _targetMask);
            foreach (var col in colliders)
            {
                if (!col.TryGetComponent(out Enemy enemy))
                    continue;

                Vector3 dir = enemy.transform.position - _body.position;
                
                Vector3 flatDir = new Vector3(dir.x, 0f, dir.z);
                float angle = Vector3.Angle(_body.forward, flatDir);
                float distance = flatDir.magnitude;
                
                float height = enemy.transform.position.y - _body.position.y;
                if (angle > _viewAngle / 2f || distance > _range || height > _heightAbove || height < -_heightBelow)
                    continue;
                
                Vector3 rayDir = flatDir.normalized;
                if (Physics.Raycast(_body.position, rayDir, out RaycastHit hit, distance, _obstacleMask))
                {
                    if (hit.transform != enemy.transform && !hit.transform.IsChildOf(enemy.transform))
                        continue;
                }

                return enemy;
            }

            return null;
        }

        private void OnDrawGizmosSelected()
        {
            if (!_body)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _range);

            Vector3 left = Quaternion.Euler(0, -_viewAngle / 2f, 0) * _body.forward;
            Vector3 right = Quaternion.Euler(0, _viewAngle / 2f, 0) * _body.forward;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_body.position, _body.position + left * _range);
            Gizmos.DrawLine(_body.position, _body.position + right * _range);
        }
    }
}