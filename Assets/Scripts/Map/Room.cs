using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public static List<Room> ActiveRooms = new List<Room>();
    public static List<Pawn> tempPawns = new List<Pawn>();

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
    public RectInt[] Areas = new RectInt[0];
    public List<Pawn> Pawns = new List<Pawn>();

    private void Awake()
    {
        ActiveRooms.Add(this);
    }

    private void OnDestroy()
    {
        ActiveRooms.Remove(this);
    }

    public static void AssignAllPawns()
    {
        tempPawns.AddRange(Pawn.AllAlive);

        foreach (var room in ActiveRooms)
        {
            room.AssignPawns(tempPawns);
        }

        tempPawns.Clear();
    }

    public void AssignPawns(List<Pawn> pawns)
    {
        Pawns.Clear();
        for (int i = 0; i < pawns.Count; i++)
        {
            var pawn = pawns[i];
            pawn.CurrentRoom = null;
            if (Contains(pawn.transform.position))
            {
                pawn.CurrentRoom = this;
                Pawns.Add(pawn);
                pawns.RemoveAt(i);
                i--;
            }
        }             
    }

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
                Gizmos.DrawCube(new Vector3(pos.x + 0.5f, 8f + i * 0.1f, pos.y + 0.5f), Vector3.one * 0.5f);
            }            
        }
    }
}
