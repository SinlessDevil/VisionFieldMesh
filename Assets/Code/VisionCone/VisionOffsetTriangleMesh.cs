using UnityEngine;

namespace Code.VisionCone
{
    public class VisionOffsetTriangleMesh : BaseVisionMesh
    {
        [Header("Square Settings")]
        [SerializeField] private float _width = 4f;
        [SerializeField] private float _height = 4f;
        [SerializeField] private int _segments = 64;
        [Header("Center Offset (local)")]
        [SerializeField] private Vector3 _centerOffset = new(0f, 0f, -2f);

        private float _lastWidth;
        private float _lastHeight;
        private int _lastSegments;
        private Vector3 _lastOffset;

        protected override string MeshName => "VisionOffsetTriangleMesh";

        protected override void GenerateMesh()
        {
            _vertices.Clear();
            _triangles.Clear();
            _normals.Clear();
            _uv.Clear();

            Vector3 origin = transform.position + transform.rotation * _centerOffset;
            Quaternion inverseRotation = Quaternion.Inverse(transform.rotation);

            _vertices.Add(_centerOffset);
            _normals.Add(Vector3.up);
            _uv.Add(Vector2.zero);

            for (int i = 0; i <= _segments; i++)
            {
                float t = i / (float)_segments;
                float localX = Mathf.Lerp(-_width / 2f, _width / 2f, t);
                Vector3 localPoint = new Vector3(localX, 0f, _height);
                Vector3 dir = transform.rotation * (localPoint - _centerOffset).normalized;

                float maxDist = (transform.rotation * (localPoint - _centerOffset)).magnitude;

                if (Physics.Raycast(origin, dir, out RaycastHit hit, maxDist, _obstacleMask))
                {
                    Vector3 localHit = inverseRotation * (hit.point - transform.position);
                    _vertices.Add(localHit);
                }
                else
                {
                    _vertices.Add(localPoint);
                }

                _normals.Add(Vector3.up);
                _uv.Add(new Vector2(t, 1));
            }
            
            for (int i = 1; i <= _segments; i++)
            {
                _triangles.Add(0);
                _triangles.Add(i);
                _triangles.Add(i + 1);
            }

            Mesh mesh = _meshFilter.sharedMesh;
            mesh.Clear();
            mesh.SetVertices(_vertices);
            mesh.SetTriangles(_triangles, 0);
            mesh.SetNormals(_normals);
            mesh.SetUVs(0, _uv);
        }
        
        protected override bool ParamsChanged()=>
            _lastWidth != _width ||
            _lastHeight != _height ||
            _lastSegments != _segments ||
            _lastOffset != _centerOffset ||
            base.ParamsChanged();

        protected override void CacheParams()
        {
            _lastWidth = _width;
            _lastHeight = _height;
            _lastSegments = _segments;
            _lastOffset = _centerOffset;
            base.CacheParams();
        }

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            Vector3 origin = transform.position + transform.rotation * _centerOffset;

            for (int i = 0; i <= _segments; i++)
            {
                float t = i / (float)_segments;
                float localX = Mathf.Lerp(-_width / 2f, _width / 2f, t);
                Vector3 localPoint = new Vector3(localX, 0f, _height);
                Vector3 dir = transform.rotation * (localPoint - _centerOffset).normalized;
                float maxDist = (transform.rotation * (localPoint - _centerOffset)).magnitude;

                if (Physics.Raycast(origin, dir, out RaycastHit hit, maxDist, _obstacleMask))
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(origin, hit.point);
                    Gizmos.DrawSphere(hit.point, 0.025f);
                }
                else
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(origin, origin + dir * maxDist);
                }
            }
#endif
        }
    }
}
