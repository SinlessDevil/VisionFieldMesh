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
            Collider[] colliders = Physics.OverlapSphere(transform.position, _height, _targetMask);
            foreach (var col in colliders)
            {
                if (!col.TryGetComponent(out Enemy enemy))
                    continue;

                if (!IsInsideArrow(enemy.transform.position))
                    continue;

                Vector3 dir = (enemy.transform.position - _body.position).normalized;
                float distance = Vector3.Distance(_body.position, enemy.transform.position);

                if (Physics.Raycast(_body.position, dir, out RaycastHit hit, distance, _obstacleMask))
                {
                    if (hit.transform != enemy.transform && !hit.transform.IsChildOf(enemy.transform))
                        continue;
                }

                return enemy;
            }

            return null;
        }

        private bool IsInsideArrow(Vector3 targetWorldPos)
        {
            Vector3 local = Quaternion.Inverse(Quaternion.Euler(0, _tiltAngle, 0)) * (_body.InverseTransformPoint(targetWorldPos));
            float x = local.x;
            float z = local.z;

            float halfW = _width / 2f;
            float halfH = _height / 2f;

            if (z < 0 || z > _height)
                return false;

            float normalizedZ = z / _height;
            float currentHalfWidth = Mathf.Lerp(halfW, 0, Mathf.Abs(0.5f - normalizedZ) * 2f);

            return Mathf.Abs(x) <= currentHalfWidth;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!_isShowDebug || !_body)
                return;

            Vector3 origin = _body.position;
            Quaternion tilt = Quaternion.Euler(0, _tiltAngle, 0);

            Gizmos.color = Color.yellow;

            // Отрисовка стрелки (по периметру)
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

            // Промежуточные лучи с цветами
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

            // Подсветка целей
            Collider[] colliders = Physics.OverlapSphere(origin, _height, _targetMask);
            foreach (var col in colliders)
            {
                if (!col.TryGetComponent(out Enemy enemy))
                    continue;

                if (!IsInsideArrow(enemy.transform.position))
                    continue;

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(enemy.transform.position, 0.3f);
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
