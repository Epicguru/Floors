using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Door))]
public class DoorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        Door d = target as Door;

        if (GUILayout.Button($"Preview camera"))
        {
            var sv = SceneView.lastActiveSceneView;
            if (sv != null)
            {
                sv.LookAt(d.CenterPos, Quaternion.Euler(d.CameraAngles), 10f);
            }
        }

        DrawDefaultInspector();
    }
}