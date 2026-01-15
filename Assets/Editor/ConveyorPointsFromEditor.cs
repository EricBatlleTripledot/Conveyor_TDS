using UnityEditor;
using UnityEngine;

namespace Game.MeshGeneration
{
    [CustomEditor(typeof(ConveyorFromPoints))]
    public class ConveyorPointsFromEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var conveyor = (ConveyorFromPoints)target;

            EditorGUILayout.Space(10);

            using (new EditorGUILayout.VerticalScope())
            {
                if (GUILayout.Button("1. Load Points From JSON"))
                {
                    conveyor.ReadConveyorPointsFromJson();
                    EditorUtility.SetDirty(conveyor);
                }

                EditorGUILayout.Space(6);
                if (GUILayout.Button("2. Build Conveyor Mesh From Points"))
                {
                    conveyor.BuildConveyorFromPoints();
                    EditorUtility.SetDirty(conveyor);
                }
            }
        }
    }
}