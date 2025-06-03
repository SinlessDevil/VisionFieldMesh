using UnityEngine;
using Sirenix.OdinInspector;

namespace Code.VisionCone.Detectors
{
    public class TargetRhombusDetector : BaseTargetDetector
    {
        [BoxGroup("Rhombus Detector Settings")] [SerializeField] private float _sideLength = 2f;
        [BoxGroup("Rhombus Detector Settings")] [SerializeField, Range(10f, 170f)] private float _angleDegrees = 60f;
        [BoxGroup("Rhombus Detector Settings")] [SerializeField, Range(4, 256)] private int _segments = 64;
        [BoxGroup("Rhombus Detector Settings")] [SerializeField, Min(0f)] private float _raycastOffset = 0.5f;

        protected override Enemy FindVisibleTarget()
        {
            Vector3 origin = _body.position;

            float angleRad = _angleDegrees * Mathf.Deg2Rad;
            float halfDiagH = Mathf.Cos(angleRad / 2f) * _sideLength;
            float halfDiagV = Mathf.Sin(angleRad / 2f) * _sideLength;

            Enemy closestEnemy = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < _segments; i += 4)
            {
                float t = i / (float)_segments;
                Vector3 edgeLocal = GetPointOnRhombusEdge(t, halfDiagH, halfDiagV);
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

        private Vector3 GetPointOnRhombusEdge(float t, float halfDiagH, float halfDiagV)
        {
            float total = t * 4f;

            return total switch
            {
                < 1f => Vector3.Lerp(new Vector3(0, 0, -halfDiagV), new Vector3(halfDiagH, 0, 0), total),
                < 2f => Vector3.Lerp(new Vector3(halfDiagH, 0, 0), new Vector3(0, 0, halfDiagV), total - 1f),
                < 3f => Vector3.Lerp(new Vector3(0, 0, halfDiagV), new Vector3(-halfDiagH, 0, 0), total - 2f),
                _    => Vector3.Lerp(new Vector3(-halfDiagH, 0, 0), new Vector3(0, 0, -halfDiagV), total - 3f),
            };
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!_isShowDebug || !_body)
                return;

            Vector3 origin = _body.position;

            float angleRad = _angleDegrees * Mathf.Deg2Rad;
            float halfDiagH = Mathf.Cos(angleRad / 2f) * _sideLength;
            float halfDiagV = Mathf.Sin(angleRad / 2f) * _sideLength;

            for (int i = 0; i <= _segments; i++)
            {
                float t = i / (float)_segments;
                Vector3 edgeLocal = GetPointOnRhombusEdge(t, halfDiagH, halfDiagV);
                Vector3 edgeWorld = _body.TransformPoint(edgeLocal);
                Vector3 dir = (edgeWorld - origin).normalized;
                float dist = Vector3.Distance(origin, edgeWorld);

                // Ищем цель
                if (Physics.Raycast(origin, dir, out RaycastHit targetHit, dist, _targetMask))
                {
                    // Проверяем, заблокирована ли цель
                    if (!Physics.Raycast(origin, dir, out RaycastHit blockHit, dist, _obstacleMask) ||
                        blockHit.transform == targetHit.transform ||
                        blockHit.transform.IsChildOf(targetHit.transform))
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(origin, targetHit.point);
                        Gizmos.DrawSphere(targetHit.point, 0.025f);
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                        Vector3 adjusted = origin + dir * Mathf.Max(0f, blockHit.distance - _raycastOffset);
                        Gizmos.DrawLine(origin, adjusted);
                        Gizmos.DrawSphere(adjusted, 0.025f);
                    }
                }
                else if (Physics.Raycast(origin, dir, out RaycastHit obstacleHit, dist, _obstacleMask))
                {
                    Gizmos.color = Color.red;
                    Vector3 adjusted = origin + dir * Mathf.Max(0f, obstacleHit.distance - _raycastOffset);
                    Gizmos.DrawLine(origin, adjusted);
                    Gizmos.DrawSphere(adjusted, 0.025f);
                }
                else
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(origin, edgeWorld);
                }
            }

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(origin, 0.04f);
        }
#endif
    }
}
