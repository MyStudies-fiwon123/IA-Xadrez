using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    private void Awake()
    {
        movement = new KingMovement();
    }

    public override AffectedPiece CreateAffected()
    {
        AffectedKingRook aff = new AffectedKingRook();
        aff.wasMoved = wasMoved;
        return aff;
    }
}
