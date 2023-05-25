using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Door))]
public class DoorEditor : Editor {

    public override void OnInspectorGUI() {

        base.OnInspectorGUI();


        Door door = (Door)target;

        EditorGUI.BeginChangeCheck();

        FireEvent(door);
        NeedsKey(door);

        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(door);
        }
    }

    private void NeedsKey(Door door) {

        bool needsKey = door.GetNeedsKey();
        if (!needsKey)
            return;

        GameObject assignedObject = (GameObject)EditorGUILayout.ObjectField("Key GameObject", door.GetKeyObject(), typeof(GameObject), true);
        door.SetKeyObject(assignedObject);
    }

    private void FireEvent(Door door) {

        bool fireEvent = door.GetFireEvent();
        if (!fireEvent)
            return;

        Transform assignedTransform = (Transform)EditorGUILayout.ObjectField("Jumpscare Transform", door.GetJumpscareTransform(), typeof(Transform), true);
        door.SetJumpscarePosition(assignedTransform);
    }
}
