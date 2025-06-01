using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.VisionCone
{
    [ExecuteAlways]
    public abstract class BaseVisionMesh : MonoBehaviour, IVisionMeshGenerator
    {
        [BoxGroup("Base Vision Settings")] [SerializeField] protected MeshRenderer _meshRenderer;
        [BoxGroup("Base Vision Settings")] [SerializeField] protected MeshFilter _meshFilter;
        [BoxGroup("Base Vision Settings")] [SerializeField] protected LayerMask _obstacleMask = ~0;
        [BoxGroup("Base Vision Settings")] [SerializeField] protected LayerMask _targetMask = ~0;
        [BoxGroup("Base Vision Settings")] [SerializeField] protected Material _coneMaterial;
        [BoxGroup("Base Vision Settings")] [SerializeField] protected int _sortOrder = 1;
        [BoxGroup("Base Vision Settings")] [SerializeField] protected int _precision = 300;

        protected float _lastAngle;
        protected float _lastRange;
        protected int _lastPrecision;
        protected Vector3 _lastPosition;
        protected Quaternion _lastRotation;

        protected readonly List<Vector3> _vertices = new();
        protected readonly List<int> _triangles = new();
        protected readonly List<Vector3> _normals = new();
        protected readonly List<Vector2> _uv = new();
        
        public virtual string MeshName => "VisionMesh";
        
        protected abstract void GenerateMesh();
        
        protected virtual void InitMesh()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            if (_meshRenderer == null)
            {
                _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            }

            _meshFilter = GetComponent<MeshFilter>();
            if (_meshFilter == null)
            {
                _meshFilter = gameObject.AddComponent<MeshFilter>();
            }
            
            if (_coneMaterial != null)
            {
                _meshRenderer.sharedMaterial = _coneMaterial;
            }

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
        }
        
        protected virtual void Update()
        {
            if (HasInitMesh())
                return;

            if (!ParamsChanged())
                return;

            GenerateMesh();
            CacheParams();
        }

        protected virtual bool HasInitMesh() => 
            _meshFilter == null || 
            _meshFilter.sharedMesh == null || 
            _meshRenderer == null;

        protected virtual bool ParamsChanged() => 
            _lastPosition != transform.position ||
            _lastRotation != transform.rotation;

        protected virtual void CacheParams()
        {
            _lastPosition = transform.position;
            _lastRotation = transform.rotation;
        }
        
        protected virtual void CleanLists()
        {
            _vertices.Clear();
            _triangles.Clear();
            _normals.Clear();
            _uv.Clear();
        }
        
        private void CleanMesh()
        {
            if (_meshRenderer != null)
                DestroyImmediate(_meshRenderer);
            if (_meshFilter != null)
                DestroyImmediate(_meshFilter);
        }
        
        [Button(ButtonSizes.Large)]
        [GUIColor(0.2f, 0.85f, 0.2f)] 
        public void CreateMesh()
        {
            if (!HasInitMesh())
            {
                Debug.Log("Mesh is Initialized!");
                return;
            }
            
            InitMesh();
            GenerateMesh();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
        }

        [Button(ButtonSizes.Large)]
        [GUIColor(0.9f, 0.3f, 0.3f)] 
        public void DeleteMesh()
        {
            CleanMesh();
            CleanLists();
            CacheParams();
        }
    }
}
