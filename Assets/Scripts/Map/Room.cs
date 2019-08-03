using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private static Color[] colours = new Color[]
    {
        Color.green,
        Color.blue,
        Color.yellow,
        Color.cyan,
        Color.magenta,
        Color.black
    };

    public string Name = "Room #0";
    [HideInInspector]
    public RectInt[] Areas = new RectInt[]
    {
        new RectInt(0, 0, 10, 10)
    };

    public bool Contains(Vector3 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.z;
        Vector2Int point = new Vector2Int(x, y);

        foreach (var area in Areas)
        {
            if (area.Contains(point))
                return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        int i = 0;
        foreach (var area in Areas)
        {
            Gizmos.color = colours[i++];
            foreach (var pos in area.allPositionsWithin)
            {
                Gizmos.DrawCube(new Vector3(pos.x + 0.5f, 8f, pos.y + 0.5f), Vector3.one * 0.5f);
            }            
        }
    }
}
