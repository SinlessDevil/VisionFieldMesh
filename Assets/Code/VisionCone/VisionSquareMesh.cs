using UnityEngine;

namespace Code.VisionCone
{
    public class VisionSquareMesh : BaseVisionMesh
    {
        [Header("Square Settings")]
        [SerializeField] private float _width = 2f;
        [SerializeField] private float _height = 2f;
        [SerializeField] private int _segments = 64;

        private float _lastWidth;
        private float _lastHeight;
        private int _lastSegments;

        protected override string MeshName => "VisionSquareMesh";

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

            for (int i = 0; i < _segments; i++)
            {
                float t = i / (float)_segments;
                Vector3 edgeLocal = GetPointOnRectangleEdge(t);
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
        
        private Vector3 GetPointOnRectangleEdge(float t)
        {
            float halfW = _width / 2f;
            float halfH = _height / 2f;

            float total = t * 4f;

            return total switch
            {
                < 1f => new Vector3(Mathf.Lerp(-halfW, halfW, total), 0, -halfH),
                < 2f => new Vector3(halfW, 0, Mathf.Lerp(-halfH, halfH, total - 1f)),
                _ => total < 3f
                    ? new Vector3(Mathf.Lerp(halfW, -halfW, total - 2f), 0, halfH)
                    : new Vector3(-halfW, 0, Mathf.Lerp(halfH, -halfH, total - 3f))
            };
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 origin = transform.position;

            for (int i = 0; i < _segments; i++)
            {
                float t = i / (float)_segments;
                Vector3 edgeLocal = GetPointOnRectangleEdge(t);
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
        }
#endif

        protected override bool ParamsChanged() =>
            _lastWidth != _width ||
            _lastHeight != _height ||
            _lastSegments != _segments ||
            base.ParamsChanged();

        protected override void CacheParams()
        {
            _lastWidth = _width;
            _lastHeight = _height;
            _lastSegments = _segments;
            base.CacheParams();
        }
    }
}
