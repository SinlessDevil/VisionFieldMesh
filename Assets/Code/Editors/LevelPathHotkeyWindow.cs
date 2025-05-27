using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Code.Editors
{
    public class LevelPathHotkeyEditorWindow : OdinEditorWindow
    {
        private const string InsertKeyPrefsKey = "ColliderMesh_InsertKey";
        private const string DeleteKeyPrefsKey = "ColliderMesh_DeleteKey";

        [MenuItem("Tools/Level Path Hotkeys Settings")]
        private static void OpenWindow() => GetWindow<LevelPathHotkeyEditorWindow>().Show();

        [Title("Level Path Hotkey Settings")]
        [PropertyOrder(0)]
        [LabelText("Add Point Key")]
        [OnValueChanged("SaveInsertKey")]
        public KeyCode InsertKey = KeyCode.Q;

        [LabelText("Delete Point Key")]
        [OnValueChanged("SaveDeleteKey")]
        public KeyCode DeleteKey = KeyCode.E;

        protected override void OnEnable()
        {
            InsertKey = (KeyCode)EditorPrefs.GetInt(InsertKeyPrefsKey, (int)KeyCode.Q);
            DeleteKey = (KeyCode)EditorPrefs.GetInt(DeleteKeyPrefsKey, (int)KeyCode.E);
        }

        private void SaveInsertKey() => EditorPrefs.SetInt(InsertKeyPrefsKey, (int)InsertKey);
        private void SaveDeleteKey() => EditorPrefs.SetInt(DeleteKeyPrefsKey, (int)DeleteKey);
    }   
}