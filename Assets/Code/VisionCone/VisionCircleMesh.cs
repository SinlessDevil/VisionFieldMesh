using System.Collections.Generic;
using UnityEngine;

namespace Code.VisionCone
{
    public class VisionCircleMesh : BaseVisionMesh
    {
        [SerializeField] protected float _visionAngle = 360f;
        [SerializeField] protected float _visionRange = 1f;
        
        private Vector3[] _precomputedDirs;

        protected override string MeshName => "VisionCircleMesh";

        protected override void GenerateMesh()
        {
            if (_precomputedDirs == null || _precomputedDirs.Length == 0)
                RecalculateDirections();

            _vertices.Clear();
            _triangles.Clear();
            _normals.Clear();
            _uv.Clear();

            _vertices.Add(Vector3.zero);
            _normals.Add(Vector3.up);
            _uv.Add(new Vector2(0.5f, 0.5f));

            Vector3 origin = transform.position;
            Quaternion rotation = transform.rotation;
            Quaternion inverse = Quaternion.Inverse(rotation);

            for (int i = 0; i < _precomputedDirs.Length; i++)
            {
                Vector3 localDir = _precomputedDirs[i].normalized;
                Vector3 worldDir = rotation * localDir;
                float distance = _visionRange;

                if (Physics.Raycast(origin, worldDir, out RaycastHit hit, _visionRange, _obstacleMask))
                {
                    distance = hit.distance;
                }

                Vector3 worldPoint = origin + worldDir * distance;
                Vector3 localPoint = inverse * (worldPoint - origin);

                _vertices.Add(localPoint);
                _normals.Add(Vector3.up);

                Vector2 uv = new Vector2(localPoint.x / (_visionRange * 2f) + 0.5f,
                    localPoint.z / (_visionRange * 2f) + 0.5f);
                _uv.Add(uv);
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
            if (mesh == null || mesh.name != MeshName)
            {
                mesh = new Mesh { name = MeshName };
                mesh.MarkDynamic();
                _meshFilter.sharedMesh = mesh;
            }
            
            mesh.Clear();
            mesh.SetVertices(_vertices);
            mesh.SetTriangles(_triangles, 0);
            mesh.SetNormals(_normals);
            mesh.SetUVs(0, _uv);
        }
        
        protected override bool ParamsChanged() =>
            !Mathf.Approximately(_lastAngle, _visionAngle) ||
            !Mathf.Approximately(_lastRange, _visionRange) ||
            _lastPrecision != _precision ||
            base.ParamsChanged();

        protected override void CacheParams()
        {
            _lastAngle = _visionAngle;
            _lastRange = _visionRange;
            _lastPrecision = _precision;
            base.CacheParams();
        }
        
        private void RecalculateDirections()
        {
            int minmax = Mathf.RoundToInt(_visionAngle / 2f);
            float step = Mathf.Clamp(_visionAngle / _precision, 0.01f, minmax);

            List<Vector3> dirs = new();
            for (float i = -minmax; i <= minmax; i += step)
            {
                float angle = (i + 90f) * Mathf.Deg2Rad;
                dirs.Add(new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)));
            }

            _precomputedDirs = dirs.ToArray();
        }
        
        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            Vector3 origin = transform.position;
            Quaternion rotation = transform.rotation;

            int minmax = Mathf.RoundToInt(_visionAngle / 2f);
            float step = Mathf.Clamp(_visionAngle / _precision, 0.01f, minmax);

            for (float i = -minmax; i <= minmax; i += step)
            {
                float angleRad = (i + 90f) * Mathf.Deg2Rad;
                float cos = Mathf.Cos(angleRad);
                float sin = Mathf.Sin(angleRad);

                Vector3 dirLocal = new Vector3(cos, 0f, sin);
                Vector3 dirWorld = rotation * dirLocal;

                if (Physics.Raycast(origin, dirWorld, out RaycastHit hit, _visionRange, _obstacleMask.value))
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(origin, origin + dirWorld * hit.distance);
                }
                else
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(origin, origin + dirWorld * _visionRange);
                }
            }
#endif
        }
    }
}
