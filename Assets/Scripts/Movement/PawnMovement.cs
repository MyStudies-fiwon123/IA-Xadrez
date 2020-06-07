using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnMovement : Movement
{
	public override List<Tile> GetValidMoves() {
		Vector2Int direction = GetDirection();

        int limit = 1;
        if (!Board.instance.selectedPiece.wasMoved)
            limit = 2;

        List<Tile> moveable = UntilBlockedPath(direction, false, limit);
        moveable.AddRange(GetPawnAttack(direction));
		return moveable;
	}

	Vector2Int GetDirection() {
		if (StateMachineController.instance.curentlyPlaying.transform.name == "GreenPieces")
			return new Vector2Int(0, -1);
		return new Vector2Int(0, 1);
	}

    List<Tile> GetPawnAttack(Vector2Int direction){
        List<Tile> pawnAttack = new List<Tile>();
        Tile temp;
        Piece piece = Board.instance.selectedPiece;
        Vector2Int leftPos = new Vector2Int(piece.tile.pos.x - 1, piece.tile.pos.y + direction.y);
        Vector2Int rightPos = new Vector2Int(piece.tile.pos.x + 1, piece.tile.pos.y + direction.y);
        temp = GetTile(leftPos);
        if (temp != null && IsEnemy(temp)){
            pawnAttack.Add(temp);
        }
        if (temp != null && IsEnemy(temp)){
            pawnAttack.Add(temp);
        }
        return pawnAttack;
    }
}
