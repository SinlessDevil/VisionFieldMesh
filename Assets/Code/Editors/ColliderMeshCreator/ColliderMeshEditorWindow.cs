using System.Collections.Generic;
using System.Linq;
using Code.ColliderMeshCreator.Runtime;
using Plugins.ConcaveHull.Code;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Code.Editors.ColliderMeshCreator
{
    public class ColliderMeshEditorWindow : OdinEditorWindow
    {
        private const string InsertKeyPrefsKey = "ColliderMesh_InsertKey";
        private const string DeleteKeyPrefsKey = "ColliderMesh_DeleteKey";

        private static readonly Color MainColor = new(0.7f, 0.9f, 1f);
        private static readonly Color ButtonColor = new(0.3f, 0.9f, 0.4f);
        private static readonly Color CreateColor = new(0.2f, 0.6f, 1f);
        private static readonly Color ManualGenColor = new(0.4f, 0.8f, 1f);

        [MenuItem("Tools/Collider Mesh Generator Editor Window")]
        private static void OpenWindow() => GetWindow<ColliderMeshEditorWindow>().Show();

        [BoxGroup("Mesh Generation Settings"), GUIColor("MainColor")]
        [SerializeField, LabelText("YOffset")]
        private float _yOffset = 0.1f;

        [BoxGroup("Mesh Generation Settings"), GUIColor("MainColor")]
        [SerializeField, LabelText("Extrusion Thickness")]
        private float _extrusion = 1f;

        [BoxGroup("Mesh Generation Settings"), GUIColor("MainColor")]
        [SerializeField, LabelText("Debug Material")]
        private Material _debugMaterial;

        [BoxGroup("Mesh Generation Settings"), GUIColor("MainColor")]
        [SerializeField, LabelText("Smooth Outline")]
        private bool _smoothOutline = false;

        [BoxGroup("Mesh Generation Settings"), GUIColor("MainColor")]
        [SerializeField, ShowIf("_smoothOutline"), LabelText("Segments per Curve"), Range(1, 10)]
        private int _smoothSegments = 4;

        [BoxGroup("Mesh Generation Settings")]
        [GUIColor(0.7f, 0.9f, 1f)]
        [SerializeField, LabelText("Flip Face Direction")]
        private bool _flipFaces = false;
        
        [BoxGroup("Collider Mesh Generation")]
        [SerializeField, LabelText("Target Mesh Filters")]
        private List<MeshFilter> _targetMeshFilters = new();

        [BoxGroup("Collider Mesh Generation"), Space(10)]
        [SerializeField, LabelText("Concavity (-1 to 1)"), Range(-1f, 1f)]
        private float _concavity = 0.5f;

        [BoxGroup("Collider Mesh Generation")]
        [SerializeField, LabelText("Scale Factor"), MinValue(0.01f)]
        private float _scaleFactor = 1f;

        [BoxGroup("Collider Mesh Generation")]
        [SerializeField, LabelText("Y Threshold Percent (0 = top only, 1 = full range)"), Range(0f, 1f)]
        private float _yThreshold = 0.05f;

        [BoxGroup("Collider Mesh Generation"), GUIColor("ButtonColor")]
        [Button(ButtonSizes.Large)]
        private void GenerateCollider()
        {
            var worldPoints = MeshPointCollector.CollectWorldPoints(_targetMeshFilters);
            if (worldPoints.Count == 0)
            {
                Debug.LogError("No vertices found in the provided MeshFilters.");
                return;
            }

            var filteredPoints = YThresholdFilter.FilterTopPoints(worldPoints, _yThreshold);
            var nodes = filteredPoints.Select((p, i) => new Node(p.x, p.z, i)).ToList();

            Hull.CleanUp();
            Hull.SetConvexHull(nodes);
            var edges = Hull.SetConcaveHull(_concavity, _scaleFactor);

            var edgePoints = edges.BuildOutline();
            if (_smoothOutline)
                edgePoints = edgePoints.SmoothOutlineCatmullRom(_smoothSegments);

            var mesh = GenerateExtrudedMesh(edgePoints);
            CreateColliderContainer("Generated_Collider", mesh);
        }

        // ---------------- Manual Outline ----------------

        [BoxGroup("Manual Outline")]
        [SerializeField, LabelText("Line Color")]
        private Color _manualLineColor = Color.green;

        [BoxGroup("Manual Outline")]
        [SerializeField, LabelText("Point Color")]
        private Color _manualPointColor = Color.red;

        [BoxGroup("Manual Outline")]
        [SerializeField, LabelText("Point Size")]
        private float _manualPointSize = 0.2f;

        [BoxGroup("Manual Outline")]
        [SerializeField, LabelText("Insert Point Key")]
        private KeyCode _insertKey = KeyCode.Q;

        [BoxGroup("Manual Outline")]
        [SerializeField, LabelText("Delete Point Key")]
        private KeyCode _deleteKey = KeyCode.E;

        [BoxGroup("Manual Outline")]
        [Button(ButtonSizes.Large)]
        [PropertyOrder(1)]
        [GUIColor(0.2f, 0.6f, 1f)]
        private void CreateManualOutlineObject()
        {
            GameObject go = new GameObject("ManualOutlineDrawer");
            Undo.RegisterCreatedObjectUndo(go, "Create ManualOutlineDrawer");
            EditorUtility.SetDirty(go);

            if (SceneView.lastActiveSceneView != null)
            {
                Vector3 camPos = SceneView.lastActiveSceneView.camera.transform.position;
                Vector3 camForward = SceneView.lastActiveSceneView.camera.transform.forward;
                go.transform.position = camPos + camForward * 5f;
            }
            
            EditorApplication.delayCall += () =>
            {
                if (go != null)
                {
                    var drawer = go.AddComponent<ManualOutlineDrawer>();
                    var so = new SerializedObject(drawer);
                    so.FindProperty("_lineColor").colorValue = _manualLineColor;
                    so.FindProperty("_pointColor").colorValue = _manualPointColor;
                    so.FindProperty("_pointSize").floatValue = _manualPointSize;
                    so.ApplyModifiedProperties();

                    Selection.activeGameObject = go;
                }
            };
        }

        [BoxGroup("Manual Outline"), PropertyOrder(2)]
        [SerializeField, LabelText("Target Manual Outline Drawers")]
        private List<ManualOutlineDrawer> _targetsManualOutlineDrawers = new();

        [BoxGroup("Manual Outline"), PropertyOrder(3), GUIColor("ManualGenColor")]
        [Button(ButtonSizes.Large)]
        private void GenerateColliderFromManualDrawers()
        {
            var points = CollectManualWorldPoints();
            if (points.Count < 3)
            {
                Debug.LogWarning("Not enough points to create mesh.");
                return;
            }

            if (_smoothOutline)
                points = points.SmoothOutlineCatmullRom(_smoothSegments);

            var mesh = GenerateExtrudedMesh(points);
            CreateColliderContainer("Manual_Collider", mesh);
        }

        private List<Vector3> CollectManualWorldPoints()
        {
            return _targetsManualOutlineDrawers
                .Where(d => d != null && d.Points != null && d.Points.Count >= 3)
                .SelectMany(d => d.Points.Select(p => d.transform.TransformPoint(p)))
                .ToList();
        }

        private void CreateColliderContainer(string name, Mesh mesh)
        {
            var go = new GameObject(name);
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;

            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterial = _debugMaterial;
            go.AddComponent<MeshCollider>().sharedMesh = mesh;
            go.GetComponent<MeshCollider>().convex = false;
        }

        protected override void OnEnable() => LoadPrefs();
        protected override void OnDisable() => SavePrefs();

        private void LoadPrefs()
        {
            if (EditorPrefs.HasKey(InsertKeyPrefsKey))
                _insertKey = (KeyCode)EditorPrefs.GetInt(InsertKeyPrefsKey);
            if (EditorPrefs.HasKey(DeleteKeyPrefsKey))
                _deleteKey = (KeyCode)EditorPrefs.GetInt(DeleteKeyPrefsKey);
        }

        private void SavePrefs()
        {
            EditorPrefs.SetInt(InsertKeyPrefsKey, (int)_insertKey);
            EditorPrefs.SetInt(DeleteKeyPrefsKey, (int)_deleteKey);
        }

        private Mesh GenerateExtrudedMesh(List<Vector3> path)
        {
            List<Vector3> verts = new();
            List<int> tris = new();

            for (int i = 0; i < path.Count; i++)
            {
                Vector3 baseA = path[i] + Vector3.up * _yOffset;
                Vector3 baseB = path[(i + 1) % path.Count] + Vector3.up * _yOffset;
                Vector3 lowerA = baseA - Vector3.up * _extrusion;
                Vector3 lowerB = baseB - Vector3.up * _extrusion;

                int start = verts.Count;
                verts.Add(baseA);
                verts.Add(baseB);
                verts.Add(lowerA);
                verts.Add(lowerB);

                if (_flipFaces)
                {
                    tris.AddRange(new[] { start + 0, start + 1, start + 2 });
                    tris.AddRange(new[] { start + 1, start + 3, start + 2 });
                }
                else
                {
                    tris.AddRange(new[] { start + 2, start + 1, start + 0 });
                    tris.AddRange(new[] { start + 2, start + 3, start + 1 });
                }
            }

            Mesh mesh = new() { name = "ColliderMesh" };
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
