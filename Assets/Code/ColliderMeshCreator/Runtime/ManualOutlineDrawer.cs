using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Code.ColliderMeshCreator.Runtime
{
    [ExecuteAlways]
    public class ManualOutlineDrawer : MonoBehaviour
    {
        [SerializeField] private List<Vector3> _points = new();

        [Space(10), Header("Gizmos Settings")]
        [SerializeField] private Color _lineColor = Color.green;
        [SerializeField] private Color _pointColor = Color.red;
        [SerializeField] private float _pointSize = 0.2f;

        public List<Vector3> Points => _points;

        private void Reset()
        {
            _points = new List<Vector3>
            {
                new(-1, 0, -1),
                new(1, 0, -1),
                new(0, 0, 1)
            };
        }

        private void OnDrawGizmos()
        {
            if (_points == null || _points.Count < 2)
                return;

            Gizmos.color = _lineColor;
            for (int i = 0; i < _points.Count; i++)
            {
                Vector3 worldPos = transform.TransformPoint(_points[i]);
                Vector3 next = transform.TransformPoint(_points[(i + 1) % _points.Count]);
                Gizmos.DrawLine(worldPos, next);
            }

            Gizmos.color = _pointColor;
            foreach (var point in _points)
            {
                Gizmos.DrawSphere(transform.TransformPoint(point), _pointSize);
            }
        }

        [Button("Add Point")]
        private void AddPoint()
        {
            Undo.RecordObject(this, "Add Point");
            Vector3 newPoint = _points.Count > 0 ? _points[^1] + Vector3.right : Vector3.zero;
            _points.Add(newPoint);
            EditorUtility.SetDirty(this);
        }
    }
}