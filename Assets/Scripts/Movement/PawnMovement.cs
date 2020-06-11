using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnMovement : Movement
{
    Vector2Int direction;
    int promotionHeight;

    public PawnMovement(bool maxTeam)
    {
        if (maxTeam){
            direction = new Vector2Int(0, 1);
            promotionHeight = 7;
        } else {
            direction = new Vector2Int(0, -1);
            promotionHeight = 0;
        }
        
        value = 100;
    }

    public override List<AvailableMove> GetValidMoves()
    {
        List<AvailableMove> moveable = GetPawnAttack(direction);
        List<AvailableMove> moves;

        if (!Board.instance.selectedPiece.wasMoved)
        {
            moves = UntilBlockedPath(direction, false, 2);
            if (moves.Count == 2)
                moves[1] = new AvailableMove(moves[1].pos, MoveType.PawnDoubleMove);
        }
        else
        {
            moves = UntilBlockedPath(direction, false, 1);
            if (moves.Count > 0)
                moves[0] = CheckPromotion(moves[0]);
        }
        moveable.AddRange(moves);

        return moveable;
    }

    AvailableMove CheckPromotion(AvailableMove availableMove)
    {
        if (availableMove.pos.y != promotionHeight)
            return availableMove;
        return new AvailableMove(availableMove.pos, MoveType.Promotion);
    }



    List<AvailableMove> GetPawnAttack(Vector2Int direction)
    {
        List<AvailableMove> pawnAttack = new List<AvailableMove>();
        Piece piece = Board.instance.selectedPiece;
        Vector2Int leftPos = new Vector2Int(piece.tile.pos.x - 1, piece.tile.pos.y + direction.y);
        Vector2Int rightPos = new Vector2Int(piece.tile.pos.x + 1, piece.tile.pos.y + direction.y);

        GetPawnAttack(GetTile(leftPos), pawnAttack);
        GetPawnAttack(GetTile(rightPos), pawnAttack);

        return pawnAttack;
    }

    private void GetPawnAttack(Tile tile, List<AvailableMove> pawnAttack)
    {
        if (tile == null)
            return;
        if (IsEnemy(tile))
        {
            pawnAttack.Add(new AvailableMove(tile.pos, MoveType.Normal));
        }
        else if (PieceMovementState.enPassantFlag.moveType == MoveType.EnPassant && PieceMovementState.enPassantFlag.pos == tile.pos)
        {
            pawnAttack.Add(new AvailableMove(tile.pos, MoveType.EnPassant));
        }
    }
}
