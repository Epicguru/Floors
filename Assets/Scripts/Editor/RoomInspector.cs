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
        Room r = target as Room;

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
                    sv.pivot = r.transform.position;
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

        GUILayout.Space(10);
        if(GUILayout.Button("Create new area"))
        {
            RectInt[] newAreas = new RectInt[r.Areas.Length + 1];
            System.Array.Copy(r.Areas, newAreas, r.Areas.Length);
            newAreas[r.Areas.Length] = new RectInt((int)r.transform.position.x, (int)r.transform.position.y, 5, 5);
            r.Areas = newAreas;
        }
        if(r.Areas.Length == 0)
        {
            EditorGUILayout.HelpBox("No areas are defined for this room. Press the Create new area button above.", MessageType.Error);
        }


        DrawDefaultInspector();
    }
}