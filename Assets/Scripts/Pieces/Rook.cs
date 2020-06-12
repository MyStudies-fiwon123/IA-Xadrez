using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        movement = new RookMovement(maxTeam);
    }

    public override AffectedPiece CreateAffected()
    {
        AffectedKingRook aff = new AffectedKingRook();
        aff.wasMoved = wasMoved;
        return aff;
    }
}
