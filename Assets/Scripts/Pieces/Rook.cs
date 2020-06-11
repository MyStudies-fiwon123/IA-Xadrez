using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        movement = new RookMovement();
    }

    public override AffectedPiece CreateAffected()
    {
        AffectedKingRook aff = new AffectedKingRook();
        aff.wasMoved = wasMoved;
        return aff;
    }
}
