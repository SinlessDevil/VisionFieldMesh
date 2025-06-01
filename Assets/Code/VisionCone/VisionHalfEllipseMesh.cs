using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.VisionCone
{
    public class VisionHalfEllipseMesh : BaseVisionMesh
    {
        [BoxGroup("Vision Half Ellipse Mesh Settings")] [SerializeField] private float _width = 4f;
        [BoxGroup("Vision Half Ellipse Mesh Settings")] [SerializeField] private float _height = 4f;
        [BoxGroup("Vision Half Ellipse Mesh Settings")] [SerializeField] private int _segments = 64;
        [BoxGroup("Vision Half Ellipse Mesh Settings")]
        [BoxGroup("Vision Half Ellipse Mesh Settings")] [SerializeField] private Vector3 _centerOffset = new(0f, 0f, -2f);
        [BoxGroup("Vision Half Ellipse Mesh Settings")] [SerializeField] private float _preLength = 1.5f;
        [BoxGroup("Vision Half Ellipse Mesh Settings")] [SerializeField] private float _length = 2f;
        [BoxGroup("Vision Half Ellipse Mesh Settings")] [SerializeField, Min(0f)] private float _raycastOffset = 0.5f;
        
        private float _lastWidth;
        private float _lastHeight;
        private int _lastSegments;
        private Vector3 _lastOffset;

        public override string MeshName => "VisionHalfEllipseMesh";

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
                    float adjustedDist = Mathf.Max(0f, hit.distance - _raycastOffset);
                    Vector3 adjustedPoint = origin + dir * adjustedDist;
                    finalPoint = inverseRotation * (adjustedPoint - transform.position);
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
        
        protected override bool ParamsChanged() =>
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

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!enabled || _segments < 2 || HasInitMesh())
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
                    float adjustedDist = Mathf.Max(0f, hit.distance - _raycastOffset);
                    Vector3 adjustedPoint = origin + dir * adjustedDist;
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(origin, adjustedPoint);
                    Gizmos.DrawSphere(adjustedPoint, 0.025f);
                }
                else
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(origin, origin + dir * maxDist);
                }
            }

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(origin, 0.05f);
        }
#endif
    }
}
