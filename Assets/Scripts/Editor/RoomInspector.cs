using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Room))]
public class RoomInspector : Editor
{
    static Vector3 oldPivot;
    static Quaternion oldRot;
    static bool inEdit;

    public override void OnInspectorGUI()
    {
        Room d = target as Room;

        if(GUILayout.Button($"{(inEdit ? "Exit" : "Enter")} edit view"))
        {
            if (!inEdit)
            {
                SceneView sv = SceneView.lastActiveSceneView;
                if (sv != null)
                {
                    sv.orthographic = true;
                    oldPivot = sv.pivot;
                    oldRot = sv.rotation;
                    sv.pivot = d.transform.position;
                    sv.rotation = Quaternion.Euler(90f, 0f, 0f);
                    inEdit = true;
                }
            }
            else
            {
                SceneView sv = SceneView.lastActiveSceneView;
                if (sv != null)
                {
                    sv.orthographic = false;
                    sv.pivot = oldPivot;
                    sv.rotation = oldRot;
                    inEdit = false;
                }
            } 
        }

        if (inEdit)
        {
            EditorGUILayout.HelpBox("Currently in edit mode. Press space to define areas in the room.", MessageType.Info);
        }

        if (Event.current.isKey && Event.current.keyCode == KeyCode.Space)
        {

        }

        DrawDefaultInspector();
    }
}