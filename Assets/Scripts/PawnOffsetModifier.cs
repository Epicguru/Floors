
using UnityEngine;

[DefaultExecutionOrder(-100)]
[ExecuteInEditMode]
public class PawnOffsetModifier : MonoBehaviour
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

        Pawn.GraphicsPositionOffset = PositionOffset;
        Pawn.GraphicsAngleOffset = AngleOffset;             
    }
}
