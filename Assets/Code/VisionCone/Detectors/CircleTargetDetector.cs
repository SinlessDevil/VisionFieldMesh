using System;
using UnityEngine;

namespace Code.VisionCone.Detectors
{
    public class CircleTargetDetector : BaseTargetDetector
    {
        [Header("Detection Settings")] [SerializeField]
        private float range = 10f;

        [SerializeField] private float viewAngle = 90f;
        [SerializeField] private float heightAbove = 1f;
        [SerializeField] private float heightBelow = 1f;
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField] private Transform eye;

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
            Collider[] colliders = Physics.OverlapSphere(transform.position, range, targetMask);
            foreach (var col in colliders)
            {
                if (!col.TryGetComponent(out Enemy enemy))
                    continue;

                Vector3 dir = enemy.transform.position - eye.position;
                float angle = Vector3.Angle(eye.forward, new Vector3(dir.x, 0, dir.z));
                float height = enemy.transform.position.y - eye.position.y;

                bool inCone = angle < viewAngle / 2f &&
                              dir.magnitude < range &&
                              height < heightAbove && height > -heightBelow;

                if (!inCone)
                    continue;

                if (Physics.Raycast(eye.position, dir.normalized, out RaycastHit hit, dir.magnitude,
                        obstacleMask))
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
            if (!eye)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);

            Vector3 left = Quaternion.Euler(0, -viewAngle / 2f, 0) * eye.forward;
            Vector3 right = Quaternion.Euler(0, viewAngle / 2f, 0) * eye.forward;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(eye.position, eye.position + left * range);
            Gizmos.DrawLine(eye.position, eye.position + right * range);
        }
    }
}