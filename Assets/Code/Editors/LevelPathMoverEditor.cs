using Code.Levels;
using UnityEditor;
using UnityEngine;

namespace Code.Editors
{
    [CustomEditor(typeof(LevelPathMover))]
    public class LevelPathMoverEditor : Editor
    {
        private const string InsertKeyPrefsKey = "ColliderMesh_InsertKey";
        private const string DeleteKeyPrefsKey = "ColliderMesh_DeleteKey";

        private LevelPathMover _mover;
        private int _activeHandleIndex = -1;

        private KeyCode InsertKey => (KeyCode)EditorPrefs.GetInt(InsertKeyPrefsKey, (int)KeyCode.Q);
        private KeyCode DeleteKey => (KeyCode)EditorPrefs.GetInt(DeleteKeyPrefsKey, (int)KeyCode.E);

        private void OnEnable()
        {
            _mover = (LevelPathMover)target;
        }

        private void OnSceneGUI()
        {
            if (_mover.Points == null || _mover.Points.Count == 0)
                return;

            Undo.RecordObject(_mover, "Edit Path Point");

            DrawHandles();
            HandleKeyboardInput();
        }

        private void DrawHandles()
        {
            for (int i = 0; i < _mover.Points.Count; i++)
            {
                Vector3 local = _mover.Points[i];
                Vector3 world = _mover.transform.TransformPoint(local);
                Vector3 moved = Handles.PositionHandle(world, Quaternion.identity);

                if (world != moved)
                {
                    _mover.Points[i] = _mover.transform.InverseTransformPoint(moved);
                    _activeHandleIndex = i;
                    EditorUtility.SetDirty(_mover);
                }
            }
        }

        private void HandleKeyboardInput()
        {
            Event e = Event.current;
            if (e.type != EventType.KeyDown) 
                return;

            if (e.keyCode == InsertKey)
            {
                e.Use();
                InsertNewPoint();
            }
            else if (e.keyCode == DeleteKey)
            {
                e.Use();
                RemovePoint();
            }
        }

        private void InsertNewPoint()
        {
            int insertIndex = (_activeHandleIndex >= 0 && _activeHandleIndex < _mover.Points.Count)
                ? _activeHandleIndex + 1
                : _mover.Points.Count;

            Vector3 basePoint = insertIndex > 0 ? _mover.Points[insertIndex - 1] : Vector3.zero;
            _mover.Points.Insert(insertIndex, basePoint);
            _activeHandleIndex = insertIndex;

            EditorUtility.SetDirty(_mover);
            Debug.Log($"Inserted new point at index {insertIndex}");
        }

        private void RemovePoint()
        {
            if (_mover.Points.Count <= 3)
            {
                Debug.LogWarning("Cannot delete point. Minimum of 3 points required.");
                return;
            }

            if (_activeHandleIndex < 0 || _activeHandleIndex >= _mover.Points.Count)
                return;

            _mover.Points.RemoveAt(_activeHandleIndex);
            _activeHandleIndex = Mathf.Clamp(_activeHandleIndex - 1, 0, _mover.Points.Count - 1);

            EditorUtility.SetDirty(_mover);
            Debug.Log($"Removed point. New active index: {_activeHandleIndex}");
        }
    }
}
