using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Door))]
public class DoorEditor : Editor {

    public override void OnInspectorGUI() {

        base.OnInspectorGUI();


        Door door = (Door)target;

        bool fireEvent = door.GetFireEvent();
        if (!fireEvent) return;

        EditorGUI.BeginChangeCheck();

        Transform assignedTransform = (Transform)EditorGUILayout.ObjectField("Jumpscare Transform", door.GetJumpscareTransform(), typeof(Transform), true);
        door.SetJumpscarePosition(assignedTransform);

        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(door);
        }
    }
}
