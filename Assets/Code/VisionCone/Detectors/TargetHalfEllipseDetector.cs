using UnityEngine;
using Sirenix.OdinInspector;

namespace Code.VisionCone.Detectors
{
    public class TargetHalfEllipseDetector : BaseTargetDetector
    {
        [BoxGroup("Half Ellipse Detector Settings")] [SerializeField] private float _width = 4f;
        [BoxGroup("Half Ellipse Detector Settings")] [SerializeField] private float _height = 4f;
        [BoxGroup("Half Ellipse Detector Settings")] [SerializeField] private Vector3 _centerOffset = new(0f, 0f, -2f);
        [BoxGroup("Half Ellipse Detector Settings")] [SerializeField] private float _preLength = 1.5f;
        [BoxGroup("Half Ellipse Detector Settings")] [SerializeField] private float _length = 2f;
        
        protected override Enemy FindVisibleTarget()
        {
            Vector3 origin = _body.position + _body.rotation * _centerOffset;
            int segments = Mathf.Max(8, _precision);

            Enemy closestEnemy = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i <= segments; i += 4)
            {
                float t = i / (float)segments;
                Vector3 localPoint = GetPointOnEllipseEdge(t);
                Vector3 worldPoint = _body.TransformPoint(localPoint + _centerOffset);

                Vector3 dir = (worldPoint - origin).normalized;
                float distance = Vector3.Distance(origin, worldPoint);

                if (Physics.Raycast(origin, dir, out RaycastHit hit, distance, _targetMask))
                {
                    if (hit.collider.TryGetComponent(out Enemy enemy))
                    {
                        if (distance < closestDistance)
                        {
                            if (!Physics.Raycast(origin, dir, out RaycastHit blockHit, distance, _obstacleMask) ||
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

            Gizmos.color = Color.yellow;
            int segments = Mathf.Max(8, _precision);
            Vector3 prevPoint = origin;

            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector3 local = GetPointOnEllipseEdge(t);
                Vector3 world = _body.TransformPoint(local + _centerOffset);

                if (i > 0)
                    Gizmos.DrawLine(prevPoint, world);

                prevPoint = world;
            }

            for (int i = 0; i <= segments; i += 4)
            {
                float t = i / (float)segments;
                Vector3 local = GetPointOnEllipseEdge(t);
                Vector3 world = _body.TransformPoint(local + _centerOffset);

                Vector3 dir = (world - origin).normalized;
                float distance = Vector3.Distance(origin, world);

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
                    Gizmos.DrawLine(origin, world);
                }
            }
        }
#endif

        private Vector3 GetPointOnEllipseEdge(float t)
        {
            float x = Mathf.Lerp(-_width / 2f, _width / 2f, t);
            float centerOffset = Mathf.Abs(t - 0.5f) * _length;
            centerOffset = Mathf.Clamp01(centerOffset);
            float curve = Mathf.Sin((1f - centerOffset) * Mathf.PI * 0.5f);
            float z = _preLength + _height * curve;

            return new Vector3(x, 0f, z);
        }
    }
}
