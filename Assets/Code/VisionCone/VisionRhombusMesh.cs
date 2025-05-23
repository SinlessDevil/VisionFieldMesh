using UnityEngine;

namespace Code.VisionCone
{
    public class VisionRhombusMesh : BaseVisionMesh
    {
        [SerializeField] private float _sideLength = 2f;
        [SerializeField, Range(10f, 170f)] private float _angleDegrees = 60f;
        [SerializeField, Range(4, 256)] private int _segments = 64;

        private float _lastSideLength;
        private int _lastSegments;

        protected override string MeshName => "VisionRhombusMesh";

        protected override void GenerateMesh()
        {
            _vertices.Clear();
            _triangles.Clear();
            _normals.Clear();
            _uv.Clear();

            Vector3 origin = transform.position;
            Quaternion rotation = transform.rotation;
            Quaternion inverse = Quaternion.Inverse(rotation);

            _vertices.Add(Vector3.zero);
            _normals.Add(Vector3.up);
            _uv.Add(Vector2.zero);

            float angleRad = _angleDegrees * Mathf.Deg2Rad;
            float halfDiagH = Mathf.Cos(angleRad / 2f) * _sideLength;
            float halfDiagV = Mathf.Sin(angleRad / 2f) * _sideLength;

            for (int i = 0; i < _segments; i++)
            {
                float t = i / (float)_segments;
                Vector3 edgeLocal = GetPointOnRhombusEdge(t, halfDiagH, halfDiagV);
                Vector3 edgeWorld = transform.TransformPoint(edgeLocal);

                Vector3 dir = (edgeWorld - origin).normalized;
                float dist = (edgeWorld - origin).magnitude;

                if (Physics.Raycast(origin, dir, out RaycastHit hit, dist, _obstacleMask))
                {
                    edgeWorld = hit.point;
                }

                Vector3 localPoint = inverse * (edgeWorld - origin);
                _vertices.Add(localPoint);
                _normals.Add(Vector3.up);
                _uv.Add(new Vector2(t, 1f));
            }

            for (int i = 1; i < _vertices.Count - 1; i++)
            {
                _triangles.Add(0);
                _triangles.Add(i + 1);
                _triangles.Add(i);
            }

            _triangles.Add(0);
            _triangles.Add(1);
            _triangles.Add(_vertices.Count - 1);

            Mesh mesh = _meshFilter.sharedMesh;
            mesh.Clear();
            mesh.SetVertices(_vertices);
            mesh.SetTriangles(_triangles, 0);
            mesh.SetNormals(_normals);
            mesh.SetUVs(0, _uv);
        }

        private Vector3 GetPointOnRhombusEdge(float t, float halfDiagH, float halfDiagV)
        {
            float total = t * 4f;

            return total switch
            {
                < 1f => Vector3.Lerp(new Vector3(0, 0, -halfDiagV), new Vector3(halfDiagH, 0, 0), total),           // вниз → вправо
                < 2f => Vector3.Lerp(new Vector3(halfDiagH, 0, 0), new Vector3(0, 0, halfDiagV), total - 1f),        // вправо → вверх
                < 3f => Vector3.Lerp(new Vector3(0, 0, halfDiagV), new Vector3(-halfDiagH, 0, 0), total - 2f),       // вверх → влево
                _    => Vector3.Lerp(new Vector3(-halfDiagH, 0, 0), new Vector3(0, 0, -halfDiagV), total - 3f),      // влево → вниз
            };
        }
        
        protected override bool ParamsChanged() =>
            _lastSideLength != _sideLength || _lastAngle != _angleDegrees || _lastSegments != _segments || base.ParamsChanged();

        protected override void CacheParams()
        {
            _lastSideLength = _sideLength;
            _lastAngle = _angleDegrees;
            _lastSegments = _segments;
            base.CacheParams();
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!enabled || _segments < 4) 
                return;

            Vector3 origin = transform.position;
            Quaternion rotation = transform.rotation;

            float angleRad = _angleDegrees * Mathf.Deg2Rad;
            float halfDiagH = Mathf.Cos(angleRad / 2f) * _sideLength;
            float halfDiagV = Mathf.Sin(angleRad / 2f) * _sideLength;

            for (int i = 0; i <= _segments; i++)
            {
                float t = i / (float)_segments;
                Vector3 edgeLocal = GetPointOnRhombusEdge(t, halfDiagH, halfDiagV);
                Vector3 edgeWorld = transform.TransformPoint(edgeLocal);
                Vector3 dir = edgeWorld - origin;
                float dist = dir.magnitude;
                dir.Normalize();

                if (Physics.Raycast(origin, dir, out RaycastHit hit, dist, _obstacleMask))
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(origin, hit.point);
                    Gizmos.DrawSphere(hit.point, 0.025f);
                }
                else
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(origin, edgeWorld);
                }
            }

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(origin, 0.05f);
        }
#endif

    }
}
