using UnityEngine;

namespace Code.VisionCone
{
    public class VisionArrowMesh : BaseVisionMesh
    {
         [SerializeField] private float _width = 2f;
        [SerializeField] private float _height = 2f;
        [SerializeField, Range(-45, 45)] private float _tiltAngle = 0f;
        [SerializeField, Range(4, 256)] private int _segments = 64;

        private float _lastWidth;
        private float _lastHeight;
        private float _lastTilt;
        private int _lastSegments;

        protected override string MeshName => "VisionArrowMesh";

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

            float halfWidth = _width / 2f;
            float halfHeight = _height / 2f;
            
            for (int i = 0; i < _segments; i++)
            {
                float t = i / (float)_segments;
                Vector3 edgeLocal = GetPointOnRhombusEdge(t, halfWidth, halfHeight);
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

        private Vector3 GetPointOnRhombusEdge(float t, float halfWidth, float halfHeight)
        {
            float total = t * 4f;

            return total switch
            {
                < 1f => Vector3.Lerp(new Vector3(-halfWidth, 0, -0f), new Vector3(halfWidth, 0, 0f), total),             // нижняя сторона
                < 2f => Vector3.Lerp(new Vector3(halfWidth, 0, 0f), new Vector3(0f, 0, halfHeight), total - 1f),         // правая вверх
                < 3f => Vector3.Lerp(new Vector3(0f, 0, halfHeight), new Vector3(-halfWidth, 0, 0f), total - 2f),        // верх влево
                _    => Vector3.Lerp(new Vector3(-halfWidth, 0, 0f), new Vector3(-halfWidth, 0, -0f), total - 3f)        // левая вниз
            };
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!enabled || _segments < 4) 
                return;

            Vector3 origin = transform.position;
            Quaternion rotation = transform.rotation;
            Quaternion tilt = Quaternion.Euler(0f, _tiltAngle, 0f);

            float halfWidth = _width / 2f;
            float halfHeight = _height / 2f;

            for (int i = 0; i < _segments; i++)
            {
                float t = i / (float)_segments;
                Vector3 edgeLocal = GetPointOnRhombusEdge(t, halfWidth, halfHeight);
                Vector3 rotated = tilt * edgeLocal;
                Vector3 edgeWorld = transform.TransformPoint(rotated);
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

        protected override bool ParamsChanged() =>
            _lastWidth != _width ||
            _lastHeight != _height ||
            _lastTilt != _tiltAngle ||
            _lastSegments != _segments ||
            base.ParamsChanged();

        protected override void CacheParams()
        {
            _lastWidth = _width;
            _lastHeight = _height;
            _lastTilt = _tiltAngle;
            _lastSegments = _segments;
            base.CacheParams();
        }
    }
}