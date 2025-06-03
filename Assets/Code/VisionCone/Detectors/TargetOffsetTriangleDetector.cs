using UnityEngine;
using Sirenix.OdinInspector;

namespace Code.VisionCone.Detectors
{
    public class TargetOffsetTriangleDetector : BaseTargetDetector
    {
        [BoxGroup("Offset Triangle Detector Settings")] [SerializeField] private float _width = 4f;
        [BoxGroup("Offset Triangle Detector Settings")] [SerializeField] private float _height = 4f;
        [BoxGroup("Offset Triangle Detector Settings")] [SerializeField] private int _segments = 64;
        [BoxGroup("Offset Triangle Detector Settings")] [SerializeField] private Vector3 _centerOffset = new(0f, 0f, -2f);
        [BoxGroup("Offset Triangle Detector Settings")] [SerializeField] private float _raycastOffset = 0.5f;

        protected override Enemy FindVisibleTarget()
        {
            Vector3 origin = _body.position + _body.rotation * _centerOffset;
            Enemy closestEnemy = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i <= _segments; i += 4)
            {
                float t = i / (float)_segments;
                float localX = Mathf.Lerp(-_width / 2f, _width / 2f, t);
                Vector3 localPoint = new Vector3(localX, 0f, _height);
                Vector3 pointWorld = _body.TransformPoint(localPoint + _centerOffset);

                Vector3 direction = (pointWorld - origin).normalized;
                float distance = Vector3.Distance(origin, pointWorld);

                if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, _targetMask))
                {
                    if (hit.collider.TryGetComponent(out Enemy enemy))
                    {
                        if (distance < closestDistance)
                        {
                            if (!Physics.Raycast(origin, direction, out RaycastHit blockHit, distance, _obstacleMask) ||
                                blockHit.transform == enemy.transform || blockHit.transform.IsChildOf(enemy.transform))
                            {
                                closestEnemy = enemy;
                                closestDistance = distance;
                            }
                        }
                    }
                }
            }

            return closestEnemy;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!_isShowDebug || !_body)
                return;

            Vector3 origin = _body.position + _body.rotation * _centerOffset;

            for (int i = 0; i <= _segments; i++)
            {
                float t = i / (float)_segments;
                float localX = Mathf.Lerp(-_width / 2f, _width / 2f, t);
                Vector3 localPoint = new Vector3(localX, 0f, _height);
                Vector3 pointWorld = _body.TransformPoint(localPoint + _centerOffset);

                Vector3 dir = (pointWorld - origin).normalized;
                float distance = Vector3.Distance(origin, pointWorld);

                Color color = Color.green;

                if (Physics.Raycast(origin, dir, out RaycastHit hit, distance, _obstacleMask | _targetMask))
                {
                    if (hit.collider.TryGetComponent(out Enemy _))
                        color = Color.yellow;
                    else
                        color = Color.red;

                    Gizmos.color = color;
                    Gizmos.DrawLine(origin, hit.point);
                }
                else
                {
                    Gizmos.color = color;
                    Gizmos.DrawLine(origin, pointWorld);
                }
            }
        }
#endif
    }
}
