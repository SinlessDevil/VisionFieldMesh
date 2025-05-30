using UnityEditor;
using UnityEngine;

namespace Code.VisionCone.Editor
{
    [CustomEditor(typeof(BaseVisionMesh), true)]
    public class BaseVisionMeshEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BaseVisionMesh meshGen = (BaseVisionMesh)target;

            MeshFilter filter = meshGen.GetComponent<MeshFilter>();
            bool hasValidMesh = filter != null &&
                                filter.sharedMesh != null &&
                                filter.sharedMesh.name == meshGen.MeshName;

            if (!hasValidMesh)
            {
                if (GUILayout.Button("Generate Mesh"))
                {
                    meshGen.GenerateOrUpdateMesh();
                }
            }
            else
            {
                if (GUILayout.Button("Update Mesh"))
                {
                    meshGen.GenerateOrUpdateMesh();
                }
            }
        }
    }
}