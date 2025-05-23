using UnityEngine;

namespace Code.VisionCone
{
    public class VisionHalfEllipseMesh : BaseVisionMesh
    {
        [Header("Square Settings")]
        [SerializeField] private float _width = 4f;
        [SerializeField] private float _height = 4f;
        [SerializeField] private int _segments = 64;
        [Header("Center Offset (local)")]
        [SerializeField] private Vector3 _centerOffset = new(0f, 0f, -2f);
        [SerializeField] private float _preLength = 1.5f;
        [SerializeField] private float _length = 2f;
        
        private float _lastWidth;
        private float _lastHeight;
        private int _lastSegments;
        private Vector3 _lastOffset;

        protected override string MeshName => "VisionHalfEllipseMesh";

        protected override void GenerateMesh()
        {
            _vertices.Clear();
            _triangles.Clear();
            _normals.Clear();
            _uv.Clear();

            _vertices.Add(_centerOffset);
            _normals.Add(Vector3.up);
            _uv.Add(Vector2.zero);

            Vector3 origin = transform.position + transform.rotation * _centerOffset;
            Quaternion inverseRotation = Quaternion.Inverse(transform.rotation);

            for (int i = 0; i <= _segments; i++)
            {
                float t = i / (float)_segments;
                float localX = Mathf.Lerp(-_width / 2f, _width / 2f, t);
                
                float centerOffset = Mathf.Abs(t - 0.5f) * _length;
                centerOffset = Mathf.Clamp01(centerOffset);
                float curve = Mathf.Sin((1f - centerOffset) * Mathf.PI * 0.5f);
                float localZ = _preLength + _height * curve;

                Vector3 localPoint = new Vector3(localX, 0f, localZ);
                Vector3 worldPoint = transform.TransformPoint(localPoint);
                Vector3 dir = (worldPoint - origin).normalized;
                float maxDist = (worldPoint - origin).magnitude;

                Vector3 finalPoint = localPoint;

                if (Physics.Raycast(origin, dir, out RaycastHit hit, maxDist, _obstacleMask))
                {
                    finalPoint = inverseRotation * (hit.point - transform.position);
                }

                _vertices.Add(finalPoint);
                _normals.Add(Vector3.up);
                _uv.Add(new Vector2(t, 1f));
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
        
        protected override bool ParamsChanged()
        {
            return
                _lastWidth != _width ||
                _lastHeight != _height ||
                _lastSegments != _segments ||
                _lastOffset != _centerOffset ||
                base.ParamsChanged();
        }

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
            if (!enabled || _segments < 2)
                return;

            Vector3 origin = transform.position + transform.rotation * _centerOffset;

            for (int i = 0; i <= _segments; i++)
            {
                float t = i / (float)_segments;
                float localX = Mathf.Lerp(-_width / 2f, _width / 2f, t);
                
                float centerOffset = Mathf.Abs(t - 0.5f) * _length;
                centerOffset = Mathf.Clamp01(centerOffset);
                float curve = Mathf.Sin((1f - centerOffset) * Mathf.PI * 0.5f);
                float localZ = _preLength + _height * curve;

                Vector3 localPoint = new Vector3(localX, 0f, localZ);
                Vector3 worldPoint = transform.TransformPoint(localPoint);
                Vector3 dir = (worldPoint - origin).normalized;
                float maxDist = (worldPoint - origin).magnitude;

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

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(origin, 0.05f);
#endif
        }
    }
}
