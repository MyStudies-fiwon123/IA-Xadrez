using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingMovement : Movement
{
    public KingMovement(){
        value = 1000;
    }

	public override List<Tile> GetValidMoves() {
		List<Tile> moves = new List<Tile>();
        moves.AddRange(UntilBlockedPath(new Vector2Int(1, 0), true, 1));
        moves.AddRange(UntilBlockedPath(new Vector2Int(-1, 0), true, 1));

        moves.AddRange(UntilBlockedPath(new Vector2Int(0, 1), true, 1));
        moves.AddRange(UntilBlockedPath(new Vector2Int(0, -1), true, 1));

        moves.AddRange(UntilBlockedPath(new Vector2Int(1, 1), true, 1));
        moves.AddRange(UntilBlockedPath(new Vector2Int(1, -1), true, 1));
        moves.AddRange(UntilBlockedPath(new Vector2Int(-1, -1), true, 1));
        moves.AddRange(UntilBlockedPath(new Vector2Int(-1, 1), true, 1));

        SetNormalMove(moves);

        moves.AddRange(Castling());
        return moves;
	}
    List<Tile> Castling(){
        List<Tile> moves = new List<Tile>();
        if (Board.instance.selectedPiece.wasMoved)
            return moves;

        Tile temp = CheckRook(new Vector2Int(1, 0));
        if (temp != null){
            temp.moveType = MoveType.Casting;
            moves.Add(temp);
        }
        temp = CheckRook(new Vector2Int(-1, 0));
        if (temp != null){
            temp.moveType = MoveType.Casting;
            moves.Add(temp);
        }

        return moves;
    }

    private Tile CheckRook(Vector2Int direction)
    {
        Rook rook;
        Tile currentTile = GetTile(Board.instance.selectedPiece.tile.pos + direction);

        while (currentTile != null){
            if (currentTile.content != null)
                break;
            currentTile = GetTile(currentTile.pos + direction);
        }
        if (currentTile == null)
            return null;
        rook = currentTile.content as Rook;
        if (rook == null || rook.wasMoved)
            return null;
        return rook.tile;
    }
}
