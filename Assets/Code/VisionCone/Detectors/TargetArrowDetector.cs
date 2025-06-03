using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.VisionCone.Detectors
{
    public class TargetArrowDetector : BaseTargetDetector
    {
        [BoxGroup("Arrow Detector Settings")] [SerializeField] private float _width = 2f;
        [BoxGroup("Arrow Detector Settings")] [SerializeField] private float _height = 2f;
        [BoxGroup("Arrow Detector Settings")] [SerializeField, Range(-45, 45)] private float _tiltAngle = 0f;

        protected override Enemy FindVisibleTarget()
        {
            Vector3 origin = _body.position;
            Quaternion tilt = Quaternion.Euler(0, _tiltAngle, 0);

            int segments = Mathf.Max(8, _precision);
            Enemy closestEnemy = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i <= segments; i += 4)
            {
                float t = i / (float)segments;
                Vector3 pointLocal = GetPointOnRhombusEdge(t);
                Vector3 pointWorld = _body.TransformPoint(tilt * pointLocal);

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

            Vector3 origin = _body.position;
            Quaternion tilt = Quaternion.Euler(0, _tiltAngle, 0);

            Gizmos.color = Color.yellow;
            int segments = Mathf.Max(8, _precision);
            Vector3 prevPoint = origin;

            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector3 pointLocal = GetPointOnRhombusEdge(t);
                Vector3 pointWorld = _body.TransformPoint(tilt * pointLocal);

                if (i > 0)
                    Gizmos.DrawLine(prevPoint, pointWorld);

                prevPoint = pointWorld;
            }

            for (int i = 0; i <= segments; i += 4)
            {
                float t = i / (float)segments;
                Vector3 pointLocal = GetPointOnRhombusEdge(t);
                Vector3 pointWorld = _body.TransformPoint(tilt * pointLocal);

                Vector3 direction = (pointWorld - origin).normalized;
                float distance = Vector3.Distance(origin, pointWorld);

                Color color = Color.green;

                if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, _obstacleMask | _targetMask))
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

        private Vector3 GetPointOnRhombusEdge(float t)
        {
            float halfWidth = _width / 2f;
            float halfHeight = _height / 2f;
            float total = t * 4f;

            return total switch
            {
                < 1f => Vector3.Lerp(new Vector3(-halfWidth, 0, 0f), new Vector3(halfWidth, 0, 0f), total),
                < 2f => Vector3.Lerp(new Vector3(halfWidth, 0, 0f), new Vector3(0f, 0, halfHeight), total - 1f),
                < 3f => Vector3.Lerp(new Vector3(0f, 0, halfHeight), new Vector3(-halfWidth, 0, 0f), total - 2f),
                _    => Vector3.Lerp(new Vector3(-halfWidth, 0, 0f), new Vector3(-halfWidth, 0, 0f), total - 3f)
            };
        }
#endif
    }
}
