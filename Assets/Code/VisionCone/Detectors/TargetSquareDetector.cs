using UnityEngine;
using Sirenix.OdinInspector;

namespace Code.VisionCone.Detectors
{
    public class TargetSquareDetector : BaseTargetDetector
    {
        [BoxGroup("Square Detector Settings")] [SerializeField] private float _width = 2f;
        [BoxGroup("Square Detector Settings")] [SerializeField] private float _height = 2f;
        [BoxGroup("Square Detector Settings")] [SerializeField] private int _segments = 64;
        [BoxGroup("Square Detector Settings")] [SerializeField, Min(0f)] private float _raycastOffset = 0.5f;

        protected override Enemy FindVisibleTarget()
        {
            Vector3 origin = _body.position;

            Enemy closestEnemy = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < _segments; i += 4)
            {
                float t = i / (float)_segments;
                Vector3 edgeLocal = GetPointOnRectangleEdge(t);
                Vector3 edgeWorld = _body.TransformPoint(edgeLocal);

                Vector3 dir = (edgeWorld - origin).normalized;
                float distance = Vector3.Distance(origin, edgeWorld);

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

        private Vector3 GetPointOnRectangleEdge(float t)
        {
            float halfW = _width / 2f;
            float halfH = _height / 2f;
            float total = t * 4f;

            return total switch
            {
                < 1f => new Vector3(Mathf.Lerp(-halfW, halfW, total), 0, -halfH),
                < 2f => new Vector3(halfW, 0, Mathf.Lerp(-halfH, halfH, total - 1f)),
                < 3f => new Vector3(Mathf.Lerp(halfW, -halfW, total - 2f), 0, halfH),
                _ => new Vector3(-halfW, 0, Mathf.Lerp(halfH, -halfH, total - 3f))
            };
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!_isShowDebug || !_body)
                return;

            Vector3 origin = _body.position;

            for (int i = 0; i < _segments; i++)
            {
                float t = i / (float)_segments;
                Vector3 edgeLocal = GetPointOnRectangleEdge(t);
                Vector3 edgeWorld = _body.TransformPoint(edgeLocal);
                Vector3 dir = (edgeWorld - origin).normalized;
                float distance = Vector3.Distance(origin, edgeWorld);

                Color color = Color.green;

                if (Physics.Raycast(origin, dir, out RaycastHit hit, distance, _obstacleMask | _targetMask))
                {
                    if (hit.collider.TryGetComponent(out Enemy _))
                        color = Color.yellow;
                    else
                        color = Color.red;

                    Vector3 adjustedPoint = origin + dir * Mathf.Max(0f, hit.distance - _raycastOffset);
                    Gizmos.color = color;
                    Gizmos.DrawLine(origin, adjustedPoint);
                    Gizmos.DrawSphere(adjustedPoint, 0.025f);
                }
                else
                {
                    Gizmos.color = color;
                    Gizmos.DrawLine(origin, edgeWorld);
                }
            }
        }
#endif
    }
}
