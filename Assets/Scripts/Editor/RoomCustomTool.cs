
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool("Room Tool", typeof(Room))]
public class RoomCustomTool : EditorTool
{
    // Serialize this value to set a default value in the Inspector.
    [SerializeField]
    public Texture2D m_ToolIcon;

    GUIContent m_IconContent;

    void OnEnable()
    {
        m_IconContent = new GUIContent()
        {
            image = m_ToolIcon,
            text = "Room Tool",
            tooltip = "Room Tool"
        };
    }

    public override GUIContent toolbarIcon
    {
        get { return m_IconContent; }
    }

    public override void OnToolGUI(EditorWindow window)
    {
        foreach (var transform in Selection.transforms)
        {
            Room r = transform.GetComponent<Room>();
            if (r == null)
                continue;

            for (int i = 0; i < r.Areas.Length; i++)
            {
                RectInt area = r.Areas[i];

                EditorGUI.BeginChangeCheck();

                Vector3 old = new Vector3(area.xMin, 8f, area.yMin);
                Vector3 old2 = new Vector3(area.xMax, 8f, area.yMax);
                Vector3 changed = Handles.PositionHandle(old, Quaternion.identity);
                Vector3 changed2 = Handles.PositionHandle(old2, Quaternion.identity);
                Vector2 delta = new Vector2(changed.x - old.x, changed.z - old.z);
                Vector2 delta2 = new Vector2(changed2.x - old2.x, changed2.z - old2.z);

                if (EditorGUI.EndChangeCheck())
                {
                    if (delta.x <= -1f)
                    {
                        area.xMin--;
                    }
                    else if (delta.x >= 1f)
                    {
                        area.xMin++;
                    }
                    if (delta.y <= -1f)
                    {
                        area.yMin--;
                    }
                    else if (delta.y >= 1f)
                    {
                        area.yMin++;
                    }

                    if (delta2.x <= -1f)
                    {
                        area.xMax--;
                    }
                    else if (delta2.x >= 1f)
                    {
                        area.xMax++;
                    }
                    if (delta2.y <= -1f)
                    {
                        area.yMax--;
                    }
                    else if (delta2.y >= 1f)
                    {
                        area.yMax++;
                    }

                    r.Areas[i] = area;
                }                
            }            
        }
    }
}