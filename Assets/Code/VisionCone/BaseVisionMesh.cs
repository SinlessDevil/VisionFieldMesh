using System.Collections.Generic;
using UnityEngine;

namespace Code.VisionCone
{
    [ExecuteAlways]
    public abstract class BaseVisionMesh : MonoBehaviour, IVisionMeshGenerator
    {
        [Header("Vision")]
        [SerializeField] protected LayerMask _obstacleMask = ~0;

        [Header("Material")]
        [SerializeField] protected Material _coneMaterial;
        [SerializeField] protected int _sortOrder = 1;

        [Header("Optimization")]
        [SerializeField] protected int _precision = 300;

        protected MeshRenderer _meshRenderer;
        protected MeshFilter _meshFilter;

        protected bool _isInitialized;

        protected float _lastAngle;
        protected float _lastRange;
        protected int _lastPrecision;
        protected Vector3 _lastPosition;
        protected Quaternion _lastRotation;

        protected readonly List<Vector3> _vertices = new();
        protected readonly List<int> _triangles = new();
        protected readonly List<Vector3> _normals = new();
        protected readonly List<Vector2> _uv = new();

        protected virtual string MeshName => "VisionMesh";

        protected virtual void OnEnable()
        {
            if (_isInitialized)
                return;

            _meshRenderer = GetComponent<MeshRenderer>();
            if (_meshRenderer == null)
            {
                _meshRenderer = gameObject.AddComponent<MeshRenderer>();
                return;
            }

            _meshFilter = GetComponent<MeshFilter>();
            if (_meshFilter == null)
            {
                _meshFilter = gameObject.AddComponent<MeshFilter>();
                return;
            }

            if (_coneMaterial != null)
                _meshRenderer.sharedMaterial = _coneMaterial;

            _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _meshRenderer.receiveShadows = false;
            _meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            _meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            _meshRenderer.allowOcclusionWhenDynamic = false;
            _meshRenderer.sortingOrder = _sortOrder;

            if (_meshFilter.sharedMesh == null || _meshFilter.sharedMesh.name != MeshName)
            {
                _meshFilter.sharedMesh = new Mesh { name = MeshName };
                _meshFilter.sharedMesh.MarkDynamic();
            }

            _isInitialized = true;
        }

        protected virtual void Update()
        {
            if (Application.isPlaying)
                return;

            if (!_isInitialized)
                OnEnable();

            if (_meshFilter == null || _meshFilter.sharedMesh == null)
                return;

            if (!ParamsChanged())
                return;

            GenerateMesh();
            CacheParams();
        }

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying && _meshFilter != null)
            {
                GenerateMesh();
            }
        }
        
        protected virtual bool ParamsChanged() =>
            _lastPosition != transform.position ||
            _lastRotation != transform.rotation;

        protected virtual void CacheParams()
        {
            _lastPosition = transform.position;
            _lastRotation = transform.rotation;
        }

        protected abstract void GenerateMesh();
    }
}
