
using UnityEngine;

[DefaultExecutionOrder(-100)]
[ExecuteInEditMode]
public class TakedownPawnOffsetModifier : MonoBehaviour
{
    private Pawn Pawn
    {
        get
        {
            if(_pawn == null)
            {
                _pawn = GetComponentInParent<Pawn>();
            }
            return _pawn; 
        }
    }
    private Pawn _pawn;

    public Vector3 PositionOffset;
    public Vector3 AngleOffset;

    private void Update()
    {
        if (Pawn == null)
            return;
        if (Pawn.TakedownPawn == null)
            return;

        Pawn.TakedownPawn.GraphicsPositionOffset = PositionOffset;
        Pawn.TakedownPawn.GraphicsAngleOffset = AngleOffset;             
    }
}
